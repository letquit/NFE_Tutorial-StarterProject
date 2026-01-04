using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    public class MobaPrefabsAuthoring : MonoBehaviour
    {
        public GameObject Champion;
        
        public class MobaPrefabsBaker : Baker<MobaPrefabsAuthoring>
        {
            public override void Bake(MobaPrefabsAuthoring authoring)
            {
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabContainerEntity, new MobaPrefabs
                {
                    Champion = GetEntity(authoring.Champion, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
