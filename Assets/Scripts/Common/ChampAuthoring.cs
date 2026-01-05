using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// ChampAuthoring是一个用于实体转换的Authoring组件，负责定义Champ实体的结构和属性
    /// </summary>
    public class ChampAuthoring : MonoBehaviour
    {
        public float MoveSpeed;
        
        /// <summary>
        /// ChampBaker是ChampAuthoring的烘焙器，负责将Authoring组件转换为运行时的实体和组件
        /// </summary>
        public class ChampBaker : Baker<ChampAuthoring>
        { 
            /// <summary>
            /// 烘焙方法，将Authoring组件转换为实体和对应的组件
            /// </summary>
            /// <param name="authoring">ChampAuthoring组件实例</param>
            public override void Bake(ChampAuthoring authoring)
            {
                // 创建一个动态变换的实体
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                // 为实体添加ChampTag组件
                AddComponent<ChampTag>(entity);
                
                // 为实体添加NewChampTag组件
                AddComponent<NewChampTag>(entity);
                
                // 为实体添加MobaTeam组件
                AddComponent<MobaTeam>(entity);
                
                // 为实体添加URPMaterialPropertyBaseColor组件
                AddComponent<URPMaterialPropertyBaseColor>(entity);
                
                // 为实体添加ChampMoveTargetPosition组件
                AddComponent<ChampMoveTargetPosition>(entity);
                
                // 为实体添加CharacterMoveSpeed组件
                AddComponent(entity, new CharacterMoveSpeed { Value =  authoring.MoveSpeed });
            }
        }
    }
}