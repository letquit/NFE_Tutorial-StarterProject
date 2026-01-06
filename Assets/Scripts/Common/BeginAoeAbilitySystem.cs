using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// AOE能力开始系统，用于处理预测模拟中的AOE能力触发逻辑
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginAoeAbilitySystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        /// <summary>
        /// 系统更新主方法，处理AOE能力的冷却检查和实例化逻辑
        /// </summary>
        /// <param name="state">系统状态引用</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick) return;
            var currentTick = networkTime.ServerTick;

            // 遍历所有AOE方面组件并处理能力触发
            foreach (var aoe in SystemAPI.Query<AoeAspect>().WithAll<Simulate>())
            {
                var isOnCooldown = true;
                var curTargetTicks = new AbilityCooldownTargetTicks();

                // 检查批量模拟步骤中的冷却状态
                for (var i = 0u; i < networkTime.SimulationStepBatchSize; i++)
                {
                    var testTick = currentTick;
                    testTick.Subtract(i);

                    if (!aoe.CooldownTargetTicks.GetDataAtTick(testTick, out curTargetTicks))
                    {
                        curTargetTicks.AoeAbility = NetworkTick.Invalid;
                    }

                    if (curTargetTicks.AoeAbility == NetworkTick.Invalid ||
                        !curTargetTicks.AoeAbility.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }
                
                if (isOnCooldown) continue;
                
                if (aoe.ShouldAttack)
                {
                    var newAoeAbility = ecb.Instantiate(aoe.AbilityPrefab);
                    var abilityTransform = LocalTransform.FromPosition(aoe.AttackPosition);
                    ecb.SetComponent(newAoeAbility, abilityTransform);
                    ecb.SetComponent(newAoeAbility, aoe.Team);

                    // 仅在客户端设置冷却时间
                    if (state.WorldUnmanaged.IsServer()) continue;
                    var newCooldownTargetTick = currentTick;
                    newCooldownTargetTick.Add(aoe.CooldownTicks);
                    curTargetTicks.AoeAbility = newCooldownTargetTick;

                    var nextTick = currentTick;
                    nextTick.Add(1u);
                    curTargetTicks.Tick = nextTick;
                    aoe.CooldownTargetTicks.AddCommandData(curTargetTicks);
                }
            }
        }
    }
}