using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 授权类，用于定义小兵路径的编辑器数据结构
    /// </summary>
    public class MinionPathAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 顶部路径的路径点数组
        /// </summary>
        public Vector3[] TopLanePath;
        
        /// <summary>
        /// 中部路径的路径点数组
        /// </summary>
        public Vector3[] MidLanePath;
        
        /// <summary>
        /// 底部路径的路径点数组
        /// </summary>
        public Vector3[] BotLanePath;

        /// <summary>
        /// 小兵路径烘焙器，将授权数据转换为实体组件数据
        /// </summary>
        public class MinionPathBaker : Baker<MinionPathAuthoring>
        {
            /// <summary>
            /// 烘焙方法，将授权组件的数据转换为ECS实体和组件
            /// </summary>
            /// <param name="authoring">授权组件实例，包含路径点数据</param>
            public override void Bake(MinionPathAuthoring authoring)
            {
                // 创建主实体和三个路径子实体
                var entity = GetEntity(TransformUsageFlags.None);
                var topLane = CreateAdditionalEntity(TransformUsageFlags.None, false, "TopLane");
                var midLane = CreateAdditionalEntity(TransformUsageFlags.None, false, "MidLane");
                var botLane = CreateAdditionalEntity(TransformUsageFlags.None, false, "BotLane");

                // 为顶部路径创建路径点缓冲区并填充数据
                var topLanePath = AddBuffer<MinionPathPosition>(topLane);
                foreach (var pathPosition in authoring.TopLanePath)
                {
                    topLanePath.Add(new MinionPathPosition { Value = pathPosition });
                }
                
                // 为中部路径创建路径点缓冲区并填充数据
                var midLanePath = AddBuffer<MinionPathPosition>(midLane);
                foreach (var pathPosition in authoring.MidLanePath)
                {
                    midLanePath.Add(new MinionPathPosition { Value = pathPosition });
                }
                
                // 为底部路径创建路径点缓冲区并填充数据
                var botLanePath = AddBuffer<MinionPathPosition>(botLane);
                foreach (var pathPosition in authoring.BotLanePath)
                {
                    botLanePath.Add(new MinionPathPosition { Value = pathPosition });
                }
                
                // 为父实体添加路径容器组件，引用三个路径实体
                AddComponent(entity, new MinionPathContainers
                {
                    TopLane = topLane,
                    MidLane = midLane,
                    BotLane = botLane
                });
            }
        }
    }
}