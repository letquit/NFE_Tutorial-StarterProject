using Common;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 小兵生成系统，负责按照指定的时间间隔和波次在游戏世界中生成小兵
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SpawnMinionSystem : ISystem 
    {
        /// <summary>
        /// 系统创建时的初始化方法，设置系统更新所需的依赖组件
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MinionPathContainers>();
            state.RequireForUpdate<MobaPrefabs>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        /// <summary>
        /// 系统更新主方法，处理小兵生成的时间逻辑和波次管理
        /// </summary>
        /// <param name="state">系统状态引用</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var minionSpawnAspect in SystemAPI.Query<MinionSpawnAspect>())
            {
                minionSpawnAspect.DecrementTimers(deltaTime);
                if (minionSpawnAspect.ShouldSpawn)
                {
                    SpawnOnEachLane(ref state);
                    minionSpawnAspect.CountSpawnedInWave++;
                    if (minionSpawnAspect.IsWaveSpawned)
                    {
                        minionSpawnAspect.ResetMinionTimer();
                        minionSpawnAspect.ResetWaveTimer();
                        minionSpawnAspect.ResetSpawnCounter();
                    }
                    else
                    {
                        minionSpawnAspect.ResetMinionTimer();
                    }
                }
            }
        }

        /// <summary>
        /// 在每条路径上生成小兵，包括上路、中路和下路
        /// </summary>
        /// <param name="state">系统状态引用</param>
        private void SpawnOnEachLane(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            var minionPrefab = SystemAPI.GetSingleton<MobaPrefabs>().Minion;
            var pathContainers = SystemAPI.GetSingleton<MinionPathContainers>();

            var topLane = SystemAPI.GetBuffer<MinionPathPosition>(pathContainers.TopLane);
            SpawnOnLane(ecb, minionPrefab, topLane);
            
            var midLane = SystemAPI.GetBuffer<MinionPathPosition>(pathContainers.MidLane);
            SpawnOnLane(ecb, minionPrefab, midLane);
            
            var botLane = SystemAPI.GetBuffer<MinionPathPosition>(pathContainers.BotLane);
            SpawnOnLane(ecb, minionPrefab, botLane);
        }

        /// <summary>
        /// 在指定路径上生成小兵，为蓝队和红队各生成一个小兵
        /// </summary>
        /// <param name="ecb">实体命令缓冲区</param>
        /// <param name="minionPrefab">小兵预制体实体</param>
        /// <param name="curLane">当前路径的位置缓冲区</param>
        private void SpawnOnLane(EntityCommandBuffer ecb, Entity minionPrefab, DynamicBuffer<MinionPathPosition> curLane)
        {
            var newBlueMinion = ecb.Instantiate(minionPrefab);
            for (var i = 0; i < curLane.Length; i++)
            {
                ecb.AppendToBuffer(newBlueMinion, curLane[i]);
            }

            var blueSpawnTransform = LocalTransform.FromPosition(curLane[0].Value);
            ecb.SetComponent(newBlueMinion, blueSpawnTransform);
            ecb.AddComponent(newBlueMinion, new MobaTeam { Value = TeamType.Blue });
            
            var newRedMinion = ecb.Instantiate(minionPrefab);
            for (var i = curLane.Length - 1; i >= 0; i--)
            {
                ecb.AppendToBuffer(newRedMinion, curLane[i]);
            }

            var redSpawnTransform = LocalTransform.FromPosition(curLane[^1].Value);
            ecb.SetComponent(newRedMinion, redSpawnTransform);
            ecb.AddComponent(newRedMinion, new MobaTeam { Value = TeamType.Red });
        }
    }
}