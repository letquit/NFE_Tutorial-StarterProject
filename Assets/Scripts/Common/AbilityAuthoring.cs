using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 能力系统授权组件，用于定义和配置游戏中的能力相关参数
    /// </summary>
    public class AbilityAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 范围伤害能力的预制体对象
        /// </summary>
        public GameObject AoeAbility;

        /// <summary>
        /// 范围伤害能力的冷却时间（以秒为单位）
        /// </summary>
        public float AoeAbilityCooldown;

        /// <summary>
        /// 网络代码配置对象，包含客户端和服务器的配置信息
        /// </summary>
        public NetCodeConfig NetCodeConfig;
        
        /// <summary>
        /// 获取模拟时钟频率，用于将冷却时间转换为时钟周期数
        /// </summary>
        private int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        /// <summary>
        /// 能力烘焙器，负责将授权组件的数据转换为ECS实体组件
        /// </summary>
        public class AbilityBaker : Baker<AbilityAuthoring>
        { 
            /// <summary>
            /// 将授权组件的数据烘焙为ECS实体组件
            /// </summary>
            /// <param name="authoring">能力授权组件实例</param>
            public override void Bake(AbilityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = GetEntity(authoring.AoeAbility, TransformUsageFlags.Dynamic)
                });
                // 将冷却时间（秒）转换为模拟时钟周期数
                AddComponent(entity, new AbilityCooldownTicks
                {
                    AoeAbility = (uint)(authoring.AoeAbilityCooldown * authoring.SimulationTickRate)
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}