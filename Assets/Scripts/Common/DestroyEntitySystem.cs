using Common;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 销毁实体系统，用于处理带有销毁标签的实体的销毁逻辑
    /// 在预测模拟系统组中最后执行，确保在正确的时间点处理实体销毁
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitySystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RespawnEntityTag>();
            state.RequireForUpdate<MobaPrefabs>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<NetworkTime>();
        }

        /// <summary>
        /// 系统更新方法，处理带有销毁标签的实体
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            
            // 检查是否是首次完全预测tick，如果不是则直接返回
            if (!networkTime.IsFirstTimeFullyPredictingTick) return;
            var currentTick = networkTime.ServerTick;

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // 遍历所有带有DestroyEntityTag和Simulate组件的实体
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<DestroyEntityTag, Simulate>().WithEntityAccess())
            {
                // 服务器端特殊处理：检查是否为游戏结束标记实体
                if (state.World.IsServer())
                {
                    if (SystemAPI.HasComponent<GameOverOnDestroyTag>(entity))
                    {
                        var gameOverPrefab = SystemAPI.GetSingleton<MobaPrefabs>().GameOverEntity;
                        var gameOverEntity = ecb.Instantiate(gameOverPrefab);

                        var losing = SystemAPI.GetComponent<MobaTeam>(entity).Value;
                        var winning = losing == TeamType.Blue ? TeamType.Red : TeamType.Blue;
                        Debug.Log($"{winning.ToString()} Team Win!!");
                        
                        ecb.SetComponent(gameOverEntity, new WinningTeam { Value = winning });
                    }

                    // 服务器端特殊处理：检查是否为英雄单位，需要重生
                    if (SystemAPI.HasComponent<ChampTag>(entity))
                    {
                        var networkEntity = SystemAPI.GetComponent<NetworkEntityReference>(entity).Value;
                        var respawnEntity = SystemAPI.GetSingletonEntity<RespawnEntityTag>();
                        var respawnTickCount = SystemAPI.GetComponent<RespawnTickCount>(respawnEntity).Value;

                        var respawnTick = currentTick;
                        respawnTick.Add(respawnTickCount);
                        
                        ecb.AppendToBuffer(respawnEntity, new RespawnBufferElement
                        {
                            NetworkEntity = networkEntity,
                            RespawnTick = respawnTick,
                            NetworkId = SystemAPI.GetComponent<NetworkId>(networkEntity).Value
                        });
                    }
                    
                    ecb.DestroyEntity(entity);
                }
                else
                {
                    // 客户端处理：将实体移动到远距离位置而不是直接销毁
                    transform.ValueRW.Position = new float3(1000f, 1000f, 1000f);
                }
            }
        }
    }
}