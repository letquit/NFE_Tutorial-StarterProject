using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    // 瘦客户端输入系统，用于在瘦客户端模拟中生成随机移动目标位置
    // 该系统更新角色的移动目标位置，并管理输入属性中的计时器和随机数生成
    [WorldSystemFilter(WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct ThinClientInputSystem : ISystem
    {
        // 更新系统逻辑，在每一帧执行以下操作：
        // 1. 获取系统时间增量
        // 2. 遍历所有具有ChampMoveTargetPosition和ThinClientInputProperties组件的实体
        // 3. 减少计时器值
        // 4. 当计时器到期时，生成新的随机目标位置并重置计时器
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (moveTargetPosition, inputProperties) in SystemAPI
                         .Query<RefRW<ChampMoveTargetPosition>, RefRW<ThinClientInputProperties>>())
            {
                // 减少当前计时器值
                inputProperties.ValueRW.Timer -= deltaTime;
                
                // 如果计时器仍大于0，则跳过本次循环
                if (inputProperties.ValueRO.Timer > 0f) continue;

                // 生成新的随机位置作为移动目标
                var randomPosition = inputProperties.ValueRW.Random.NextFloat3(inputProperties.ValueRO.MinPosition,
                    inputProperties.ValueRO.MaxPosition);
                moveTargetPosition.ValueRW.Value = randomPosition;

                // 重置计时器为新的随机值
                inputProperties.ValueRW.Timer =
                    inputProperties.ValueRW.Random.NextFloat(inputProperties.ValueRO.MinTimer,
                        inputProperties.ValueRO.MaxTimer);
            }
        }
    }
}
