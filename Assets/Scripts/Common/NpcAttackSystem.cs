using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// NPC攻击系统，负责处理NPC的攻击逻辑和攻击实例的生成
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct NpcAttackSystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<GamePlayingTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        /// <summary>
        /// 系统更新方法，调度NPC攻击作业
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            state.Dependency = new NpcAttackJob
            {
                CurrentTick = networkTime.ServerTick,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(state.Dependency);
        }
    }

    /// <summary>
    /// NPC攻击作业，处理单个NPC的攻击逻辑
    /// </summary>
    [BurstCompile]
    [WithAll(typeof(Simulate))]
    public partial struct NpcAttackJob : IJobEntity
    {
        [ReadOnly] public NetworkTick CurrentTick;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;

        public EntityCommandBuffer.ParallelWriter ECB;

        /// <summary>
        /// 执行NPC攻击逻辑
        /// </summary>
        /// <param name="attackCooldown">NPC攻击冷却动态缓冲区</param>
        /// <param name="attackProperties">NPC攻击属性</param>
        /// <param name="targetEntity">NPC目标实体</param>
        /// <param name="npcEntity">NPC实体</param>
        /// <param name="team">Moba团队</param>
        /// <param name="sortKey">查询中的块索引</param>
        [BurstCompile]
        private void Execute(ref DynamicBuffer<NpcAttackCooldown> attackCooldown,
            in NpcAttackProperties attackProperties, in NpcTargetEntity targetEntity, Entity npcEntity, MobaTeam team,
            [ChunkIndexInQuery] int sortKey)
        {
            // 检查目标实体是否存在
            if (!TransformLookup.HasComponent(targetEntity.Value)) return;
            if (!attackCooldown.GetDataAtTick(CurrentTick, out var cooldownExpirationTick))
            {
                cooldownExpirationTick.Value = NetworkTick.Invalid;
            }

            // 检查攻击冷却是否已过期
            var canAttack = !cooldownExpirationTick.Value.IsValid ||
                            CurrentTick.IsNewerThan(cooldownExpirationTick.Value);
            if (!canAttack) return;
            
            // 计算攻击发射点位置和目标位置
            var spawnPosition = TransformLookup[npcEntity].Position + attackProperties.FirePointOffset;
            var targetPosition = TransformLookup[targetEntity.Value].Position;

            // 创建新的攻击实例
            var newAttack = ECB.Instantiate(sortKey, attackProperties.AttackPrefab);
            var newAttackTransform = LocalTransform.FromPositionRotation(spawnPosition,
                quaternion.LookRotationSafe(targetPosition - spawnPosition, math.up()));
            
            ECB.SetComponent(sortKey, newAttack, newAttackTransform);
            ECB.SetComponent(sortKey, newAttack, team);

            // 设置新的攻击冷却时间
            var newCooldownTick = CurrentTick;
            newCooldownTick.Add(attackProperties.CooldownTickCount);
            attackCooldown.AddCommandData(new NpcAttackCooldown { Tick = CurrentTick, Value = newCooldownTick });
        }
    }
}