using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 技能射击方面组件，用于处理技能射击相关的数据和逻辑
    /// </summary>
    public readonly partial struct SkillShotAspect : IAspect
    {
        /// <summary>
        /// 英雄实体
        /// </summary>
        public readonly Entity ChampionEntity;

        /// <summary>
        /// 技能输入组件的只读引用
        /// </summary>
        private readonly RefRO<AbilityInput> _abilityInput;
        
        /// <summary>
        /// 技能预制体组件的只读引用
        /// </summary>
        private readonly RefRO<AbilityPrefabs> _abilityPrefabs;
        
        /// <summary>
        /// 技能冷却时间组件的只读引用
        /// </summary>
        private readonly RefRO<AbilityCooldownTicks> _abilityCooldownTicks;
        
        /// <summary>
        /// 队伍组件的只读引用
        /// </summary>
        private readonly RefRO<MobaTeam> _mobaTeam;
        
        /// <summary>
        /// 本地变换组件的只读引用
        /// </summary>
        private readonly RefRO<LocalTransform> _localTransform;
        
        /// <summary>
        /// 技能冷却目标时间缓冲区的引用
        /// </summary>
        private readonly DynamicBuffer<AbilityCooldownTargetTicks> _abilityCooldownTargetTicks;
        
        /// <summary>
        /// 瞄准输入组件的只读引用
        /// </summary>
        private readonly RefRO<AimInput> _aimInput;

        /// <summary>
        /// 获取是否开始攻击
        /// </summary>
        public bool BeginAttack => _abilityInput.ValueRO.SkillShotAbility.IsSet;
        
        /// <summary>
        /// 获取是否确认攻击
        /// </summary>
        public bool ConfirmAttack => _abilityInput.ValueRO.ConfirmSkillShotAbility.IsSet;
        
        /// <summary>
        /// 获取技能预制体实体
        /// </summary>
        public Entity AbilityPrefab => _abilityPrefabs.ValueRO.SkillShotAbility;
        
        /// <summary>
        /// 获取当前队伍信息
        /// </summary>
        public MobaTeam Team => _mobaTeam.ValueRO;
        
        /// <summary>
        /// 获取技能冷却目标时间缓冲区
        /// </summary>
        public DynamicBuffer<AbilityCooldownTargetTicks> CooldownTargetTicks => _abilityCooldownTargetTicks;
        
        /// <summary>
        /// 获取技能冷却时间
        /// </summary>
        public uint CooldownTicks => _abilityCooldownTicks.ValueRO.SkillShotAbility;
        
        /// <summary>
        /// 获取攻击位置
        /// </summary>
        public float3 AttackPosition => _localTransform.ValueRO.Position;
        
        /// <summary>
        /// 获取生成位置，基于当前位置和瞄准方向计算
        /// </summary>
        public LocalTransform SpawnPosition => LocalTransform.FromPositionRotation(_localTransform.ValueRO.Position,
            quaternion.LookRotationSafe(_aimInput.ValueRO.Value, math.up()));
    }
}