using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// Moba游戏预制体授权类，用于在编辑器中配置游戏所需的预制体资源
    /// </summary>
    public class MobaPrefabsAuthoring : MonoBehaviour
    {
        [Header("Entities")]
        public GameObject Champion;
        public GameObject Minion;
        public GameObject GameOverEntity;
        public GameObject RespawnEntity;

        [Header("GameObjects")]
        public GameObject HealthBarPrefab;
        public GameObject SkillShotAimPrefab;
        
        /// <summary>
        /// Moba预制体烘焙器，将授权组件中的预制体配置转换为ECS实体组件
        /// </summary>
        public class MobaPrefabsBaker : Baker<MobaPrefabsAuthoring>
        {
            /// <summary>
            /// 烘焙方法，将授权组件中的预制体数据转换为ECS组件
            /// </summary>
            /// <param name="authoring">MobaPrefabsAuthoring授权组件实例，包含各种预制体的引用</param>
            public override void Bake(MobaPrefabsAuthoring authoring)
            {
                // 创建预制体容器实体
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                
                // 添加MobaPrefabs组件，包含实体类型的预制体
                AddComponent(prefabContainerEntity, new MobaPrefabs
                {
                    Champion = GetEntity(authoring.Champion, TransformUsageFlags.Dynamic),
                    Minion = GetEntity(authoring.Minion, TransformUsageFlags.Dynamic),
                    GameOverEntity = GetEntity(authoring.GameOverEntity, TransformUsageFlags.None),
                    RespawnEntity = GetEntity(authoring.RespawnEntity, TransformUsageFlags.None)
                });
                
                // 添加UIPrefabs组件对象，包含游戏对象类型的UI预制体
                AddComponentObject(prefabContainerEntity, new UIPrefabs
                {
                    HealthBar = authoring.HealthBarPrefab,
                    SkillShot = authoring.SkillShotAimPrefab
                });
            }
        }
    }
}