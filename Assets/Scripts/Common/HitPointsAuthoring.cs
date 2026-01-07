using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// HitPointsAuthoring类用于在Unity编辑器中定义实体的生命值相关属性
    /// 该类继承自MonoBehaviour，作为Authoring组件用于将数据转换为ECS实体
    /// </summary>
    public class HitPointsAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 最大生命值，定义实体的初始最大生命值
        /// </summary>
        public int MaxHitPoints;

        /// <summary>
        /// 健康条的偏移量向量，用于定义健康条相对于目标对象的位置偏移
        /// </summary>
        public Vector3 HealthBarOffset;

        /// <summary>
        /// HitPointsBaker类负责将HitPointsAuthoring组件烘焙为ECS实体及其相关组件
        /// </summary>
        public class HitPointsBaker : Baker<HitPointsAuthoring>
        {
            /// <summary>
            /// 将HitPointsAuthoring组件的数据烘焙为ECS实体组件
            /// </summary>
            /// <param name="authoring">HitPointsAuthoring组件实例，包含烘焙所需的数据</param>
            public override void Bake(HitPointsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CurrentHitPoints { Value = authoring.MaxHitPoints });
                AddComponent(entity, new MaxHitPoints { Value = authoring.MaxHitPoints });
                AddBuffer<DamageBufferElement>(entity);
                AddBuffer<DamageThisTick>(entity);
                AddComponent(entity, new HealthBarOffset { Value = authoring.HealthBarOffset });
            }
        }
    }
}