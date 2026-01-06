using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 初始化销毁定时器系统，负责将DestroyOnTimer组件转换为DestroyAtTick组件
    /// </summary>
    public partial struct InitializeDestroyOnTimerSystem : ISystem
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
        /// 系统更新方法，处理需要销毁的实体并设置销毁时间
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            // 遍历所有具有DestroyOnTimer组件但没有DestroyAtTick组件的实体
            foreach (var (destroyOnTimer, entity) in SystemAPI.Query<DestroyOnTimer>().WithNone<DestroyAtTick>().WithEntityAccess())
            {
                var lifetimeInTicks = (uint)(destroyOnTimer.Value * simulationTickRate);
                var targetTick = currentTick;
                targetTick.Add(lifetimeInTicks);
                ecb.AddComponent(entity, new DestroyAtTick { Value = targetTick });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}