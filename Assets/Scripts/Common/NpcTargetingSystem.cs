using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// NPC目标选择系统，负责为NPC实体找到最近的可攻击目标
    /// </summary>
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(ExportPhysicsWorld))]
    public partial struct NpcTargetingSystem : ISystem
    {
        private CollisionFilter _npcAttackFilter;

        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            // 配置NPC攻击的碰撞过滤器，定义哪些层可以被检测到
            _npcAttackFilter = new CollisionFilter
            {
                BelongsTo = 1 << 6, // Target Cast - NPC的检测层
                CollidesWith = 1 << 1 | 1 << 2 | 1 << 4, // Champions, Minions, Structures - 可攻击的目标层
            };
        }
        
        /// <summary>
        /// 系统更新方法，调度NPC目标选择作业
        /// </summary>
        /// <param name="state">系统状态引用</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new NpcTargetingJob
            {
                CollisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
                CollisionFilter = _npcAttackFilter,
                MobaTeamLookup = SystemAPI.GetComponentLookup<MobaTeam>(true)
            }.ScheduleParallel(state.Dependency);
        }
    }

    /// <summary>
    /// NPC目标选择作业，执行具体的碰撞检测和目标选择逻辑
    /// </summary>
    [BurstCompile]
    [WithAll(typeof(Simulate))]
    public partial struct NpcTargetingJob : IJobEntity
    {
        [ReadOnly] public CollisionWorld CollisionWorld;
        [ReadOnly] public CollisionFilter CollisionFilter;
        [ReadOnly] public ComponentLookup<MobaTeam> MobaTeamLookup;

        /// <summary>
        /// 执行NPC目标选择逻辑
        /// </summary>
        /// <param name="npcEntity">当前处理的NPC实体</param>
        /// <param name="targetEntity">NPC的目标实体组件引用</param>
        /// <param name="transform">NPC的本地变换组件</param>
        /// <param name="targetRadius">NPC的目标检测半径</param>
        [BurstCompile]
        private void Execute(Entity npcEntity, ref NpcTargetEntity targetEntity, in LocalTransform transform,
            in NpcTargetRadius targetRadius)
        {
            var hits = new NativeList<DistanceHit>(Allocator.TempJob);

            // 在NPC周围的目标检测半径内进行球体重叠检测
            if (CollisionWorld.OverlapSphere(transform.Position, targetRadius.Value, ref hits, CollisionFilter))
            {
                var closestDistance = float.MaxValue;
                var closestEntity = Entity.Null;

                // 遍历所有检测到的碰撞体，找到最近的有效目标
                foreach (var hit in hits)
                {
                    if (!MobaTeamLookup.TryGetComponent(hit.Entity, out var mobaTeam)) continue;
                    // 跳过同队的实体
                    if (mobaTeam.Value == MobaTeamLookup[npcEntity].Value) continue;
                    if (hit.Distance < closestDistance)
                    {
                        closestDistance = hit.Distance;
                        closestEntity = hit.Entity;
                    }
                }

                targetEntity.Value = closestEntity;
            }
            else
            {
                targetEntity.Value = Entity.Null;
            }
            
            hits.Dispose();
        }
    }
}