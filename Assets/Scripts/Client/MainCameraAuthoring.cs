using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 主相机的Authoring组件，用于在编辑器中配置主相机的属性和设置
    /// </summary>
    public class MainCameraAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 主相机的Baker类，负责将Authoring组件转换为运行时的ECS实体组件
        /// </summary>
        public class MainCameraBaker : Baker<MainCameraAuthoring>
        {
            /// <summary>
            /// 将Authoring组件烘焙为ECS实体和组件
            /// </summary>
            /// <param name="authoring">MainCameraAuthoring类型的Authoring组件实例</param>
            public override void Bake(MainCameraAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new MainCamera());
                AddComponent<MainCameraTag>(entity);
            }
        }
    }
}