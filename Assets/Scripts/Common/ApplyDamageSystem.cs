using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 应用伤害系统，负责处理实体的伤害计算和销毁逻辑
    /// </summary>
    /// <remarks>
    /// 该系统在预测模拟系统组中最后执行，并在计算帧伤害系统之后执行
    /// </remarks>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(CalculateFrameDamageSystem))]
    public partial struct ApplyDamageSystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<GamePlayingTag>();
        }

        /// <summary>
        /// 系统更新方法，处理伤害应用和实体销毁逻辑
        /// </summary>
        /// <param name="state">系统状态引用</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // 遍历所有具有当前生命值组件和伤害缓冲区的模拟实体
            foreach (var (currentHitPoints, damageThisTickBuffer, entity) in SystemAPI
                         .Query<RefRW<CurrentHitPoints>, DynamicBuffer<DamageThisTick>>().WithAll<Simulate>()
                         .WithEntityAccess())
            {
                if (!damageThisTickBuffer.GetDataAtTick(currentTick, out var damageThisTick)) continue;
                if (damageThisTick.Tick != currentTick) continue;
                currentHitPoints.ValueRW.Value -= damageThisTick.Value;

                // 检查实体生命值是否归零或小于零，如果是则标记为销毁
                if (currentHitPoints.ValueRO.Value <= 0)
                {
                    ecb.AddComponent<DestroyEntityTag>(entity);
                }
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}