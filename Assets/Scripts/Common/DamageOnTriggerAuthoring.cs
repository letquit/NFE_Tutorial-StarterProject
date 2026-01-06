using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 作者化组件，用于在Unity编辑器中设置触发器伤害值
    /// </summary>
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 触发器伤害值
        /// </summary>
        public int DamageOnTrigger;
        
        /// <summary>
        /// 将作者化组件转换为ECS实体的烘焙器
        /// </summary>
        public class DamageOnTriggerBaker : Baker<DamageOnTriggerAuthoring>
        {
            /// <summary>
            /// 将作者化组件数据烘焙到ECS实体中
            /// </summary>
            /// <param name="authoring">作者化组件实例</param>
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageOnTrigger { Value = authoring.DamageOnTrigger });
                AddBuffer<AlreadyDamagedEntity>(entity);
            }
        }
    }
}