using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 负责配置实体重生相关参数的授权组件
    /// </summary>
    public class RespawnEntityAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 重生时间（以秒为单位）
        /// </summary>
        public float RespawnTime;

        /// <summary>
        /// 网络代码配置对象
        /// </summary>
        public NetCodeConfig NetCodeConfig;
        
        /// <summary>
        /// 获取模拟 Tick 速率
        /// </summary>
        public int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        /// <summary>
        /// 将授权数据转换为 ECS 实体的烘焙器
        /// </summary>
        public class RespawnEntityBaker : Baker<RespawnEntityAuthoring>
        { 
            /// <summary>
            /// 将授权组件的数据烘焙到 ECS 实体中
            /// </summary>
            /// <param name="authoring">授权组件实例</param>
            public override void Bake(RespawnEntityAuthoring authoring)
            {
                // 创建实体并添加重生相关的组件和缓冲区
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<RespawnEntityTag>(entity);
                AddComponent(entity, new RespawnTickCount
                {
                    Value = (uint)(authoring.RespawnTime * authoring.SimulationTickRate)
                });
                AddBuffer<RespawnBufferElement>(entity);
            }
        }
    }
}