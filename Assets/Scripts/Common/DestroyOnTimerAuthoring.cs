using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 用于在指定时间后销毁实体的Authoring组件
    /// </summary>
    public class DestroyOnTimerAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 销毁计时器值，指定实体在多少秒后被销毁
        /// </summary>
        public float DestroyOnTimer;

        /// <summary>
        /// 将DestroyOnTimerAuthoring组件转换为ECS实体的Baker类
        /// </summary>
        public class DestroyOnTimerBaker : Baker<DestroyOnTimerAuthoring>
        {
            /// <summary>
            /// 将Authoring组件烘焙为ECS实体和组件
            /// </summary>
            /// <param name="authoring">DestroyOnTimerAuthoring组件实例</param>
            public override void Bake(DestroyOnTimerAuthoring authoring)
            {
                // 创建动态实体并添加销毁计时器组件
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DestroyOnTimer { Value = authoring.DestroyOnTimer });
            }
        }
    }
}