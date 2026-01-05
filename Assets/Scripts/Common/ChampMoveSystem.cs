using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 角色移动系统，负责处理角色向目标位置的移动逻辑
    /// 该系统在预测模拟系统组中更新，用于处理网络预测的角色移动
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct ChampMoveSystem : ISystem
    {
        /// <summary>
        /// 系统更新方法，处理所有需要移动的角色实体
        /// </summary>
        /// <param name="state">系统状态引用，提供对ECS框架的访问</param>
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            // 遍历所有具有移动组件且需要模拟的实体
            foreach (var (transform, movePosition, moveSpeed, entity) in SystemAPI
                         .Query<RefRW<LocalTransform>, RefRO<ChampMoveTargetPosition>, RefRO<CharacterMoveSpeed>>()
                         .WithAll<Simulate>()
                         .WithEntityAccess())
            {
                var moveTarget = movePosition.ValueRO.Value;
                moveTarget.y = transform.ValueRO.Position.y;

                // 检查是否已到达目标位置，如果距离小于阈值则跳过移动
                if (math.distancesq(transform.ValueRO.Position, moveTarget) < 0.001f) 
                    continue;

                // 计算移动方向并更新角色位置
                var moveDirection = math.normalize(moveTarget - transform.ValueRO.Position);
                var moveVector = moveDirection * moveSpeed.ValueRO.Value * deltaTime;
                transform.ValueRW.Position += moveVector;
                
                // 更新角色朝向，使其面向移动方向
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(moveDirection, math.up());
            }
        }
    }
}