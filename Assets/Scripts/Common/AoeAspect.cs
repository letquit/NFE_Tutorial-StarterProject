using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// AoeAspect 是一个只读的部分结构体，实现了 IAspect 接口，用于处理范围攻击（Area of Effect）相关的组件数据访问
    /// </summary>
    public readonly partial struct AoeAspect : IAspect
    {
        private readonly RefRO<AbilityInput> _abilityInput;
        private readonly RefRO<AbilityPrefabs> _abilityPrefabs;
        private readonly RefRO<MobaTeam> _mobaTeam;
        private readonly RefRO<LocalTransform> _localTransform;
        private readonly RefRO<AbilityCooldownTicks> _abilityCooldownTicks;
        private readonly DynamicBuffer<AbilityCooldownTargetTicks> _abilityCooldownTargetTicks;
        
        /// <summary>
        /// 获取是否应该执行攻击操作
        /// </summary>
        /// <returns>当 AoeAbility 被设置时返回 true，否则返回 false</returns>
        public bool ShouldAttack => _abilityInput.ValueRO.AoeAbility.IsSet;
        
        /// <summary>
        /// 获取范围攻击的预制体实体
        /// </summary>
        /// <returns>范围攻击能力的预制体实体</returns>
        public Entity AbilityPrefab => _abilityPrefabs.ValueRO.AoeAbility;
        
        /// <summary>
        /// 获取当前实体的团队信息
        /// </summary>
        /// <returns>当前实体所属的 MobaTeam</returns>
        public MobaTeam Team => _mobaTeam.ValueRO;
        
        /// <summary>
        /// 获取攻击位置
        /// </summary>
        /// <returns>当前实体的本地变换位置</returns>
        public float3 AttackPosition => _localTransform.ValueRO.Position;
        
        /// <summary>
        /// 获取技能冷却时间
        /// </summary>
        /// <returns>范围攻击能力的冷却时间（以tick为单位）</returns>
        public uint CooldownTicks => _abilityCooldownTicks.ValueRO.AoeAbility;
        
        /// <summary>
        /// 获取技能目标冷却时间的动态缓冲区
        /// </summary>
        /// <returns>包含目标冷却时间信息的动态缓冲区</returns>
        public DynamicBuffer<AbilityCooldownTargetTicks> CooldownTargetTicks => _abilityCooldownTargetTicks;
    }
}