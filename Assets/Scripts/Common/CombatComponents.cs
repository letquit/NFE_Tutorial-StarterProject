using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 表示实体的最大生命值组件数据
    /// </summary>
    public struct MaxHitPoints : IComponentData
    {
        /// <summary>
        /// 最大生命值数值
        /// </summary>
        public int Value;
    }

    /// <summary>
    /// 表示实体当前生命值组件数据
    /// </summary>
    public struct CurrentHitPoints : IComponentData
    {
        /// <summary>
        /// 当前生命值数值，该字段会在网络同步时被序列化
        /// </summary>
        [GhostField] public int Value;
    }

    /// <summary>
    /// 损伤缓冲区元素，用于存储待处理的损伤值
    /// 该组件会在所有预测实例中同步
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct DamageBufferElement : IBufferElementData
    {
        /// <summary>
        /// 损伤值
        /// </summary>
        public int Value;
    }

    /// <summary>
    /// 表示本tick的损伤命令数据
    /// 该组件会在所有预测实例中同步，并且只发送给非所有者
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted, OwnerSendType = SendToOwnerType.SendToNonOwner)]
    public struct DamageThisTick : ICommandData
    {
        /// <summary>
        /// 网络tick信息，用于同步时间点
        /// </summary>
        public NetworkTick Tick { get; set; }
        
        /// <summary>
        /// 损伤值
        /// </summary>
        public int Value;
    }

    /// <summary>
    /// 存储能力预制体的组件数据
    /// 该结构体用于存储不同类型的能力实体引用，作为ECS组件使用
    /// </summary>
    public struct AbilityPrefabs : IComponentData
    {
        /// <summary>
        /// 范围伤害能力实体
        /// </summary>
        public Entity AoeAbility;

        /// <summary>
        /// 技能射击能力实体
        /// 存储技能射击类型能力的实体引用
        /// </summary>
        public Entity SkillShotAbility;
    }

    /// <summary>
    /// 基于计时器销毁实体的组件数据
    /// </summary>
    public struct DestroyOnTimer : IComponentData
    {
        /// <summary>
        /// 销毁倒计时值
        /// </summary>
        public float Value;
    }

    /// <summary>
    /// 在指定网络tick时销毁实体的组件数据
    /// </summary>
    public struct DestroyAtTick : IComponentData
    {
        /// <summary>
        /// 销毁实体的网络tick值
        /// </summary>
        [GhostField] public NetworkTick Value;
    } 
    
    /// <summary>
    /// 标记实体需要被销毁的组件数据
    /// </summary>
    public struct DestroyEntityTag : IComponentData {}

    /// <summary>
    /// 触发时造成伤害的组件数据
    /// </summary>
    public struct DamageOnTrigger : IComponentData
    {
        /// <summary>
        /// 造成的伤害值
        /// </summary>
        public int Value;
    }

    /// <summary>
    /// 存储已造成伤害的实体缓冲元素数据
    /// </summary>
    public struct AlreadyDamagedEntity : IBufferElementData
    {
        /// <summary>
        /// 已造成伤害的实体
        /// </summary>
        public Entity Value;
    }

    /// <summary>
    /// 能力冷却时间的组件数据
    /// 用于存储不同类型能力的冷却时间tick数
    /// </summary>
    public struct AbilityCooldownTicks : IComponentData
    {
        /// <summary>
        /// 范围伤害能力的冷却tick数
        /// </summary>
        public uint AoeAbility;
        
        /// <summary>
        /// 技能射击能力的冷却tick数
        /// </summary>
        public uint SkillShotAbility;
    }

    /// <summary>
    /// 预测能力冷却目标时间的命令数据组件
    /// 用于在网络同步中传递能力冷却相关的时间戳信息
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AbilityCooldownTargetTicks : ICommandData
    {
        /// <summary>
        /// 当前命令的网络tick
        /// 表示该命令在网络时间轴上的执行时间点
        /// </summary>
        public NetworkTick Tick { get; set; }
        
        /// <summary>
        /// 范围伤害能力的网络tick
        /// 记录范围伤害技能的冷却完成时间点
        /// </summary>
        public NetworkTick AoeAbility;
        
        /// <summary>
        /// 技能射击能力的网络tick
        /// 记录技能射击类型的冷却完成时间点
        /// </summary>
        public NetworkTick SkillShotAbility;
    }
    
    /// <summary>
    /// 表示瞄准技能射击的标签组件，用于标识实体具有瞄准射击能力
    /// </summary>
    public struct AimSkillShotTag : IComponentData {}

    /// <summary>
    /// 表示移动速度能力组件，存储实体的移动速度值
    /// </summary>
    public struct AbilityMoveSpeed : IComponentData
    {
        /// <summary>
        /// 移动速度的数值
        /// </summary>
        public float Value;
    }

    /// <summary>
    /// NPC目标检测半径组件数据
    /// </summary>
    public struct NpcTargetRadius : IComponentData
    {
        /// <summary>
        /// 目标检测半径值
        /// </summary>
        public float Value;
    }

    /// <summary>
    /// NPC当前目标实体组件数据
    /// </summary>
    public struct NpcTargetEntity : IComponentData
    {
        /// <summary>
        /// 当前目标实体引用，用于网络同步
        /// </summary>
        [GhostField] public Entity Value;
    }

    /// <summary>
    /// NPC攻击属性组件数据
    /// </summary>
    public struct NpcAttackProperties : IComponentData
    {
        /// <summary>
        /// 攻击发射点相对于NPC位置的偏移量
        /// </summary>
        public float3 FirePointOffset;
        
        /// <summary>
        /// 攻击冷却时间（以游戏tick为单位）
        /// </summary>
        public uint CooldownTickCount;
        
        /// <summary>
        /// 攻击预制体实体引用
        /// </summary>
        public Entity AttackPrefab;
    }

    /// <summary>
    /// NPC攻击冷却命令数据
    /// </summary>
    public struct NpcAttackCooldown : ICommandData
    {
        /// <summary>
        /// 网络同步tick时间戳
        /// </summary>
        public NetworkTick Tick { get; set; }
        
        /// <summary>
        /// 攻击冷却结束的tick值
        /// </summary>
        public NetworkTick Value;
    }
    
    /// <summary>
    /// 游戏结束标记组件 - 当带有此组件的游戏对象被销毁时触发游戏结束逻辑
    /// </summary>
    public struct GameOverOnDestroyTag : IComponentData {}

}