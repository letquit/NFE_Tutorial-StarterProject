using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 计算帧伤害系统，用于在预测模拟系统组中计算每个tick的总伤害值
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    public partial struct CalculateFrameDamageSystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        /// <summary>
        /// 系统更新方法，计算当前tick的总伤害并清空伤害缓冲区
        /// </summary>
        /// <param name="state">系统状态引用</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            // 遍历所有具有Simulate组件的实体，处理其伤害缓冲区
            foreach (var (damageBuffer, damageThisTickBuffer) in SystemAPI
                         .Query<DynamicBuffer<DamageBufferElement>, DynamicBuffer<DamageThisTick>>()
                         .WithAll<Simulate>())
            {
                if (damageBuffer.IsEmpty)
                {
                    damageThisTickBuffer.AddCommandData(new DamageThisTick { Tick = currentTick, Value = 0 });
                }
                else
                {
                    var totalDamage = 0;
                    if (damageThisTickBuffer.GetDataAtTick(currentTick, out var damageThisTick))
                    {
                        totalDamage = damageThisTick.Value;
                    }

                    // 累加当前缓冲区中的所有伤害值
                    foreach (var damage in damageBuffer)
                    {
                        totalDamage += damage.Value;
                    }

                    // 将总伤害值添加到当前tick的伤害记录中，并清空原始伤害缓冲区
                    damageThisTickBuffer.AddCommandData(new DamageThisTick { Tick = currentTick, Value = totalDamage });
                    damageBuffer.Clear();
                }
            }
        }
    }
}