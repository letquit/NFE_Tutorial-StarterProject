using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 移动小兵系统，负责处理小兵沿着预设路径的移动逻辑
    /// </summary>
    /// <remarks>
    /// 该系统在预测模拟系统组中更新，使用Burst编译优化性能
    /// </remarks>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct MoveMinionSystem : ISystem
    {
        /// <summary>
        /// 当系统创建时调用的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用，用于配置系统更新条件</param>
        public void OnCreate(ref SystemState state)
        {
            // 配置系统更新条件：只有当存在GamePlayingTag组件的实体时才执行此系统
            state.RequireForUpdate<GamePlayingTag>();
        }

        /// <summary>
        /// 系统更新方法，处理所有需要移动的小兵实体
        /// </summary>
        /// <param name="state">系统状态引用，提供对ECS框架的访问</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            // 遍历所有具有变换组件、路径位置缓冲区、路径索引和移动速度的模拟实体
            foreach (var (transform, pathPositions, pathIndex, moveSpeed) in SystemAPI
                         .Query<RefRW<LocalTransform>, DynamicBuffer<MinionPathPosition>, RefRW<MinionPathIndex>,
                             CharacterMoveSpeed>().WithAll<Simulate>())
            {
                var curTargetPosition = pathPositions[pathIndex.ValueRO.Value].Value;
                
                // 检查是否到达当前目标位置，如果是则移动到下一个路径点
                if (math.distance(curTargetPosition, transform.ValueRO.Position) <= 1.5)
                {
                    if (pathIndex.ValueRO.Value >= pathPositions.Length - 1) continue;
                    pathIndex.ValueRW.Value++;
                    curTargetPosition = pathPositions[pathIndex.ValueRO.Value].Value;
                }

                // 保持Y轴高度不变，计算移动方向并更新位置和旋转
                curTargetPosition.y = transform.ValueRO.Position.y;
                var curHeading = math.normalizesafe(curTargetPosition - transform.ValueRO.Position);
                transform.ValueRW.Position += curHeading * moveSpeed.Value * deltaTime;
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(curHeading, math.up());
            }
        }
    }
}
