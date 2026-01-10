using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// Authoring组件，用于在游戏对象被销毁时触发游戏结束逻辑
    /// </summary>
    public class GameOverOnDestroyAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 将GameOverOnDestroyAuthoring转换为ECS实体的烘焙器
        /// </summary>
        public class GameOverOnDestroyBaker : Baker<GameOverOnDestroyAuthoring>
        {
            /// <summary>
            /// 将Authoring组件烘焙为ECS实体，添加GameOverOnDestroyTag组件
            /// </summary>
            /// <param name="authoring">GameOverOnDestroyAuthoring类型的Authoring组件实例</param>
            public override void Bake(GameOverOnDestroyAuthoring authoring)
            {
                // 创建动态实体并添加GameOverOnDestroyTag组件
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<GameOverOnDestroyTag>(entity);
            }
        }
    }
}