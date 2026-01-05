using Common;
using TMG.NFE_Tutorial;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Server
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
            // 确保系统更新时需要MobaPrefabs组件
            state.RequireForUpdate<MobaPrefabs>();
            
            // 构建查询条件：同时具有MobaTeamRequest和ReceiveRpcCommandRequest组件的实体
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<MobaTeamRequest, ReceiveRpcCommandRequest>();
            
            // 设置系统更新时需要满足查询条件的实体存在
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }
        
        /// <summary>
        /// 系统更新方法，处理游戏加入请求
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var championPrefab = SystemAPI.GetSingleton<MobaPrefabs>().Champion;
            foreach (var (teamRequest, requestSource, requestEntity) in SystemAPI
                         .Query<MobaTeamRequest, ReceiveRpcCommandRequest>().WithEntityAccess())
            {
                ecb.DestroyEntity(requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

                var requestedTeamType = teamRequest.Value;

                // 自动分配队伍处理：如果请求为自动分配，则默认分配到蓝色队伍
                if (requestedTeamType == TeamType.AutoAssign)
                {
                    requestedTeamType = TeamType.Blue;
                }

                var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
                
                Debug.Log($"Server is assigning Client ID: {clientId} to the {requestedTeamType.ToString()} team.");
                
                float3 spawnPosition;

                // 根据队伍类型设置不同的出生位置
                switch (requestedTeamType)
                {
                    case TeamType.Blue:
                        spawnPosition = new float3(-50f, 1f, -50f);
                        break;
                    case TeamType.Red:
                        spawnPosition = new float3(50f, 1f, 50f);
                        break;
                    default:
                        continue;
                }

                var newChamp = ecb.Instantiate(championPrefab);
                ecb.SetName(newChamp, "Champion");
                
                var newTransform = LocalTransform.FromPosition(spawnPosition);
                ecb.SetComponent(newChamp, newTransform);
                ecb.SetComponent(newChamp, new GhostOwner { NetworkId = clientId });
                ecb.SetComponent(newChamp, new MobaTeam { Value = requestedTeamType });
                
                // 设置 Champion 的移动目标位置为当前的出生位置
                ecb.SetComponent(newChamp, new ChampMoveTargetPosition { Value = spawnPosition });
                
                ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = newChamp });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}