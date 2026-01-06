using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 技能移动速度能力的授权组件，用于在Unity编辑器中设置移动速度值
    /// </summary>
    public class AbilityMoveSpeedAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 能力移动速度值
        /// </summary>
        public float AbilityMoveSpeed;

        /// <summary>
        /// AbilityMoveSpeedAuthoring的烘焙器类，负责将授权组件转换为ECS实体组件
        /// </summary>
        public class AbilityMoveSpeedBaker : Baker<AbilityMoveSpeedAuthoring>
        {
            /// <summary>
            /// 将授权组件烘焙为ECS实体组件
            /// </summary>
            /// <param name="authoring">授权组件实例</param>
            public override void Bake(AbilityMoveSpeedAuthoring authoring)
            {
                // 创建实体并设置为动态变换使用标志
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                // 为实体添加移动速度组件并赋值
                AddComponent(entity, new AbilityMoveSpeed { Value = authoring.AbilityMoveSpeed });
            }
        }
    }
}