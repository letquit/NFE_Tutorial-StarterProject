using Common;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 服务器处理游戏加入请求系统，负责处理客户端的队伍分配请求
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntryRequestSystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<GameStartProperties>();
            // 确保系统更新时需要MobaPrefabs组件
            state.RequireForUpdate<MobaPrefabs>();
            
            // 构建查询条件：同时具有MobaTeamRequest和ReceiveRpcCommandRequest组件的实体
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<MobaTeamRequest, ReceiveRpcCommandRequest>();
            
            // 设置系统更新时需要满足查询条件的实体存在
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }
        
        /// <summary>
        /// 系统更新方法，处理游戏加入请求并管理玩家队伍分配、角色实例化和游戏启动逻辑
        /// 该方法遍历所有MobaTeamRequest RPC请求，根据请求的队伍类型分配玩家到相应队伍，
        /// 实例化英雄角色，设置出生位置，并管理游戏开始倒计时
        /// </summary>
        /// <param name="state">系统状态引用，用于访问EntityManager和其他系统API组件</param>
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var championPrefab = SystemAPI.GetSingleton<MobaPrefabs>().Champion;

            var gamePropertyEntity = SystemAPI.GetSingletonEntity<GameStartProperties>();
            var gameStartProperties = SystemAPI.GetComponent<GameStartProperties>(gamePropertyEntity);
            var teamPlayerCounter = SystemAPI.GetComponent<TeamPlayerCounter>(gamePropertyEntity);
            var spawnOffsets = SystemAPI.GetBuffer<SpawnOffset>(gamePropertyEntity);
            
            foreach (var (teamRequest, requestSource, requestEntity) in SystemAPI
                         .Query<MobaTeamRequest, ReceiveRpcCommandRequest>().WithEntityAccess())
            {
                ecb.DestroyEntity(requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

                var requestedTeamType = teamRequest.Value;

                // 自动分配队伍处理：如果请求为自动分配，则默认分配到蓝色队伍
                if (requestedTeamType == TeamType.AutoAssign)
                {
                    if (teamPlayerCounter.BlueTeamPlayers > teamPlayerCounter.RedTeamPlayers)
                    {
                        requestedTeamType = TeamType.Red;
                    }
                    else if (teamPlayerCounter.BlueTeamPlayers <= teamPlayerCounter.RedTeamPlayers)
                    {
                        requestedTeamType = TeamType.Blue;
                    }
                }

                var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
                float3 spawnPosition;

                // 根据队伍类型设置不同的出生位置
                switch (requestedTeamType)
                {
                    case TeamType.Blue:
                        if (teamPlayerCounter.BlueTeamPlayers >= gameStartProperties.MaxPlayersPerTeam)
                        {
                            Debug.Log($"Blue Team is full. Client ID: {clientId} is spectating the game");
                            continue;
                        }
                        spawnPosition = new float3(-50f, 1f, -50f);
                        spawnPosition += spawnOffsets[teamPlayerCounter.BlueTeamPlayers].Value;
                        teamPlayerCounter.BlueTeamPlayers++;
                        break;
                    case TeamType.Red:
                        if (teamPlayerCounter.RedTeamPlayers >= gameStartProperties.MaxPlayersPerTeam)
                        {
                            Debug.Log($"Red Team is full. Client ID: {clientId} is spectating the game");
                            continue;
                        }
                        spawnPosition = new float3(50f, 1f, 50f);
                        spawnPosition += spawnOffsets[teamPlayerCounter.RedTeamPlayers].Value;
                        teamPlayerCounter.RedTeamPlayers++;
                        break;
                    default:
                        continue;
                }
                
                Debug.Log($"Server is assigning Client ID: {clientId} to the {requestedTeamType.ToString()} team.");

                var newChamp = ecb.Instantiate(championPrefab);
                ecb.SetName(newChamp, "Champion");
                
                var newTransform = LocalTransform.FromPosition(spawnPosition);
                ecb.SetComponent(newChamp, newTransform);
                ecb.SetComponent(newChamp, new GhostOwner { NetworkId = clientId });
                ecb.SetComponent(newChamp, new MobaTeam { Value = requestedTeamType });
                
                ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = newChamp });

                ecb.SetComponent(newChamp, new NetworkEntityReference { Value = requestSource.SourceConnection });
                
                ecb.AddComponent(requestSource.SourceConnection, new PlayerSpawnInfo
                {
                    MobaTeam = requestedTeamType,
                    SpawnPosition = spawnPosition
                });
                
                ecb.SetComponent(requestSource.SourceConnection, new CommandTarget { targetEntity = newChamp });
                
                // 游戏开始条件检查和倒计时管理
                // 检查当前玩家数量是否满足最小启动要求，并且游戏尚未开始
                var playersRemainingToStart =
                    gameStartProperties.MinPlayersToStartGame - teamPlayerCounter.TotalPlayers;

                var gameStartRpc = ecb.CreateEntity();
                if (playersRemainingToStart <= 0 && !SystemAPI.HasSingleton<GamePlayingTag>())
                {
                    var simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
                    var ticksUntilStart = (uint)(simulationTickRate * gameStartProperties.CountdownTime);
                    var gameStartTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
                    gameStartTick.Add(ticksUntilStart);
                    
                    ecb.AddComponent(gameStartRpc, new GameStartTickRpc
                    {
                        Value = gameStartTick
                    });
                    
                    var gameStartEntity = ecb.CreateEntity();
                    ecb.AddComponent(gameStartEntity, new GameStartTick
                    {
                        Value = gameStartTick
                    });
                }
                else
                {
                    ecb.AddComponent(gameStartRpc, new PlayersRemainingToStart { Value = playersRemainingToStart });
                }
                ecb.AddComponent<SendRpcCommandRequest>(gameStartRpc);
            }
            
            ecb.Playback(state.EntityManager);
            SystemAPI.SetSingleton(teamPlayerCounter);
        }
    }
}