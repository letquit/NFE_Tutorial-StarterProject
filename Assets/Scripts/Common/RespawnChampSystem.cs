using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 负责处理英雄重生逻辑的系统，在预测模拟系统组中更新
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class RespawnChampSystem : SystemBase
    {
        /// <summary>
        /// 重生倒计时更新事件回调
        /// </summary>
        public Action<int> OnUpdateRespawnCountdown;
        
        /// <summary>
        /// 重生完成事件回调
        /// </summary>
        public Action OnRespawn;
        
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        protected override void OnCreate()
        {
            RequireForUpdate<NetworkTime>();
            RequireForUpdate<MobaPrefabs>();
        }

        /// <summary>
        /// 系统开始运行时的初始化方法
        /// </summary>
        protected override void OnStartRunning()
        {
            // 检查是否存在重生实体标签，如果不存在则创建重生实体
            if (SystemAPI.HasSingleton<RespawnEntityTag>()) return;
            var respawnPrefab = SystemAPI.GetSingleton<MobaPrefabs>().RespawnEntity;
            EntityManager.Instantiate(respawnPrefab);
        }

        /// <summary>
        /// 系统主更新循环，处理重生缓冲区中的重生逻辑
        /// </summary>
        protected override void OnUpdate()
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick) return;
            var currentTick = networkTime.ServerTick;
            
            var isServer = World.IsServer();

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // 遍历所有具有重生缓冲区、重生刻度计数和模拟组件的实体
            foreach (var respawnBuffer in SystemAPI.Query<DynamicBuffer<RespawnBufferElement>>()
                         .WithAll<RespawnTickCount, Simulate>())
            {
                var respawnsToCleanup = new NativeList<int>(Allocator.Temp); 
                
                // 遍历当前实体的重生缓冲区中的所有重生元素
                for (var i = 0; i < respawnBuffer.Length; i++)
                {
                    var curRespawn = respawnBuffer[i];

                    // 检查当前网络刻度是否达到或超过重生刻度
                    if (currentTick.Equals(curRespawn.RespawnTick) || currentTick.IsNewerThan(curRespawn.RespawnTick))
                    {
                        if (isServer)
                        {
                            // 服务器端重生逻辑：实例化新的英雄并设置相关组件
                            var networkId = SystemAPI.GetComponent<NetworkId>(curRespawn.NetworkEntity).Value;
                            var playerSpawnInfo = SystemAPI.GetComponent<PlayerSpawnInfo>(curRespawn.NetworkEntity);

                            var championPrefab = SystemAPI.GetSingleton<MobaPrefabs>().Champion;
                            var newChamp = ecb.Instantiate(championPrefab);

                            ecb.SetComponent(newChamp, new GhostOwner { NetworkId = networkId });
                            ecb.SetComponent(newChamp, new MobaTeam { Value = playerSpawnInfo.MobaTeam });
                            ecb.SetComponent(newChamp, LocalTransform.FromPosition(playerSpawnInfo.SpawnPosition));
                            ecb.SetComponent(newChamp, new ChampMoveTargetPosition
                            {
                                Value = playerSpawnInfo.SpawnPosition
                            });
                            ecb.AppendToBuffer(curRespawn.NetworkEntity, new LinkedEntityGroup { Value = newChamp });
                            ecb.SetComponent(newChamp, new NetworkEntityReference { Value = curRespawn.NetworkEntity });
                            
                            respawnsToCleanup.Add(i);
                        }
                        else
                        {
                            // 客户端重生完成回调
                            OnRespawn?.Invoke();
                        }
                    }
                    else if (!isServer)
                    {
                        // 客户端倒计时更新逻辑
                        if (SystemAPI.TryGetSingleton<NetworkId>(out var networkId))
                        {
                            if (networkId.Value == curRespawn.NetworkId)
                            {
                                var ticksToRespawn = curRespawn.RespawnTick.TickIndexForValidTick -
                                                     currentTick.TickIndexForValidTick;
                                var simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
                                var secondsToStart = (int)math.ceil((float)ticksToRespawn / simulationTickRate);
                                OnUpdateRespawnCountdown?.Invoke(secondsToStart);
                            }
                        }
                    }
                }

                // 清理已完成重生的缓冲区元素
                foreach (var respawnIndex in respawnsToCleanup)
                {
                    respawnBuffer.RemoveAt(respawnIndex);
                }
            }
            
            ecb.Playback(EntityManager);
        }
    }
}