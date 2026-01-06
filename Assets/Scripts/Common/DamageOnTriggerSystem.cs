using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 触发器伤害系统，负责处理物理触发器事件并应用伤害
    /// </summary>
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct DamageOnTriggerSystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        /// <summary>
        /// 系统更新方法，调度触发器伤害作业
        /// </summary>
        /// <param name="state">系统状态引用</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var damageOnTriggerJob = new DamageOnTriggerJob
            {
                DamageOnTriggerLookup = SystemAPI.GetComponentLookup<DamageOnTrigger>(true),
                TeamLookup = SystemAPI.GetComponentLookup<MobaTeam>(true),
                AlreadyDamagedLookup = SystemAPI.GetBufferLookup<AlreadyDamagedEntity>(true),
                DamageBufferLookup = SystemAPI.GetBufferLookup<DamageBufferElement>(true),
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            };
            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = damageOnTriggerJob.Schedule(simulationSingleton, state.Dependency);
        }
    }

    /// <summary>
    /// 触发器伤害作业，处理物理触发器事件并应用伤害
    /// </summary>
    public struct DamageOnTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<DamageOnTrigger> DamageOnTriggerLookup;
        [ReadOnly] public ComponentLookup<MobaTeam> TeamLookup;
        [ReadOnly] public BufferLookup<AlreadyDamagedEntity> AlreadyDamagedLookup;
        [ReadOnly] public BufferLookup<DamageBufferElement> DamageBufferLookup;

        public EntityCommandBuffer ECB;
        
        /// <summary>
        /// 执行触发器事件处理
        /// </summary>
        /// <param name="triggerEvent">触发器事件数据</param>
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity damageDealingEntity;
            Entity damageReceivingEntity;

            // 确定哪个实体是伤害施加者，哪个是伤害接收者
            if (DamageBufferLookup.HasBuffer(triggerEvent.EntityA) &&
                DamageOnTriggerLookup.HasComponent(triggerEvent.EntityB))
            {
                damageReceivingEntity = triggerEvent.EntityA;
                damageDealingEntity = triggerEvent.EntityB;
            }
            else if (DamageOnTriggerLookup.HasComponent(triggerEvent.EntityA) &&
                     DamageBufferLookup.HasBuffer(triggerEvent.EntityB))
            {
                damageDealingEntity = triggerEvent.EntityA;
                damageReceivingEntity = triggerEvent.EntityB;
            }
            else
            {
                return;
            }
            
            // 防止重复伤害
            var alreadyDamagedBuffer = AlreadyDamagedLookup[damageDealingEntity];
            foreach (var alreadyDamagedEntity in alreadyDamagedBuffer)
            {
                if (alreadyDamagedEntity.Value.Equals(damageReceivingEntity)) return;
            }
            
            // 忽略友军伤害
            if (TeamLookup.TryGetComponent(damageDealingEntity, out var damageDealingTeam) &&
                TeamLookup.TryGetComponent(damageReceivingEntity, out var damageReceivingTeam))
            {
                if (damageDealingTeam.Value == damageReceivingTeam.Value) return;
            }

            var damageOnTrigger = DamageOnTriggerLookup[damageDealingEntity];
            ECB.AppendToBuffer(damageReceivingEntity, new DamageBufferElement { Value = damageOnTrigger.Value });
            ECB.AppendToBuffer(damageDealingEntity, new AlreadyDamagedEntity { Value = damageReceivingEntity });
        }
    }
}