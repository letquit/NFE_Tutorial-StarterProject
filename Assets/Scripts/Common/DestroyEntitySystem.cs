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
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
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
            var currentTIck = networkTime.ServerTick;

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // 遍历所有带有DestroyEntityTag和Simulate组件的实体
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<DestroyEntityTag, Simulate>().WithEntityAccess())
            {
                // 服务器端直接销毁实体，客户端将实体移动到远距离位置
                if (state.World.IsServer())
                {
                    ecb.DestroyEntity(entity);
                }
                else
                {
                    transform.ValueRW.Position = new float3(1000f, 1000f, 1000f);
                }
            }
        }
    }
}