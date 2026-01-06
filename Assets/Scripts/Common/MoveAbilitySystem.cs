using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 移动能力系统，负责处理具有移动能力的实体的位置更新
    /// 该系统在预测模拟系统组中运行，用于处理网络预测环境下的实体移动
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct MoveAbilitySystem : ISystem
    {
        /// <summary>
        /// 系统更新方法，处理所有具有移动能力的实体的位置更新
        /// </summary>
        /// <param name="state">系统状态引用，提供对系统上下文的访问</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            // 遍历所有具有LocalTransform组件和AbilityMoveSpeed组件且带有Simulate标签的实体
            // 计算并更新实体位置，基于移动速度、时间增量和实体当前朝向
            foreach (var (transform, moveSpeed) in SystemAPI.Query<RefRW<LocalTransform>, AbilityMoveSpeed>()
                         .WithAll<Simulate>())
            {
                transform.ValueRW.Position += transform.ValueRW.Forward() * moveSpeed.Value * deltaTime;
            }
        }
    }
}