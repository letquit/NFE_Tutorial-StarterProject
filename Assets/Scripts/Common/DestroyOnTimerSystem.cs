using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 销毁计时器系统，用于在指定的网络时间tick时销毁实体
    /// </summary>
    /// <remarks>
    /// 该系统在预测模拟系统组中更新，用于处理带有销毁计时器组件的实体
    /// </remarks>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct DestroyOnTimerSystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }
        
        /// <summary>
        /// 系统更新方法，检查并销毁到达销毁时间的实体
        /// </summary>
        /// <param name="state">系统状态引用</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            // 遍历所有带有DestroyAtTick组件、Simulate组件且没有DestroyEntityTag组件的实体
            foreach (var (destroyAtTick, entity) in SystemAPI.Query<DestroyAtTick>().WithAll<Simulate>()
                         .WithNone<DestroyEntityTag>().WithEntityAccess())
            {
                if (currentTick.Equals(destroyAtTick.Value) || currentTick.IsNewerThan(destroyAtTick.Value))
                {
                    ecb.AddComponent<DestroyEntityTag>(entity);
                }
            }
        }
    }
}