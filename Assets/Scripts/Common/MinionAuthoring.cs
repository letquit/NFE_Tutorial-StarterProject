using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 小兵实体的授权组件，用于在Unity编辑器中配置小兵的属性
    /// </summary>
    public class MinionAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 小兵的移动速度
        /// </summary>
        public float MoveSpeed;

        /// <summary>
        /// 小兵烘焙器，负责将授权组件转换为ECS实体及其组件
        /// </summary>
        public class MinionBaker : Baker<MinionAuthoring>
        {
            /// <summary>
            /// 将授权组件烘焙为ECS实体和相关组件
            /// </summary>
            /// <param name="authoring">小兵授权组件实例</param>
            public override void Bake(MinionAuthoring authoring)
            {
                // 创建动态实体并添加各种组件
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<MinionTag>(entity);
                AddComponent<NewMinionTag>(entity);
                AddComponent(entity, new CharacterMoveSpeed { Value = authoring.MoveSpeed });
                AddComponent<MinionPathIndex>(entity);
                AddBuffer<MinionPathPosition>(entity);
                AddComponent<MobaTeam>(entity);
                AddComponent<URPMaterialPropertyBaseColor>(entity);
            }
        }
    }
}