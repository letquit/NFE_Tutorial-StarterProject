using Common;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    // 简单的标签组件，用于标识游戏中的英雄单位
    public struct ChampTag : IComponentData { }
    
    // 新英雄标签组件，用于标识新创建的英雄单位
    public struct NewChampTag : IComponentData { }
    
    // 角色所属英雄标签组件，用于标识当前英雄所属的英雄单位
    public struct OwnerChampTag : IComponentData { }
    
    /// <summary>
    /// MOBA游戏中的队伍组件数据
    /// </summary>
    public struct MobaTeam : IComponentData
    {
        /// <summary>
        /// 队伍类型字段，用于网络同步
        /// </summary>
        [GhostField] public TeamType Value;
    }
    
    /// <summary>
    /// 角色移动速度组件数据
    /// </summary>
    public struct CharacterMoveSpeed : IComponentData
    {
        /// <summary>
        /// 移动速度数值
        /// </summary>
        public float Value;
    }

    /// <summary>
    /// 英雄移动目标位置组件数据，用于网络预测
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct ChampMoveTargetPosition : IInputComponentData
    {
        /// <summary>
        /// 移动目标位置坐标，无量化处理
        /// </summary>
        [GhostField(Quantization = 0)] public float3 Value;
    }

    /// <summary>
    /// 表示能力输入组件，用于处理AOE（范围效果）能力的输入事件
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AbilityInput : IInputComponentData
    {
        /// <summary>
        /// AOE能力的输入事件
        /// </summary>
        [GhostField] public InputEvent AoeAbility;
        
        /// <summary>
        /// 技能射击能力的输入事件
        /// </summary>
        [GhostField] public InputEvent SkillShotAbility;

        /// <summary>
        /// 确认技能射击能力的输入事件
        /// </summary>
        [GhostField] public InputEvent ConfirmSkillShotAbility;
    }

    /// <summary>
    /// 表示瞄准输入组件，用于处理瞄准相关的输入数据
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AimInput : IInputComponentData
    {
        /// <summary>
        /// 瞄准值，表示瞄准方向或位置的三维向量
        /// </summary>
        [GhostField(Quantization = 0)] public float3 Value;
    }
}