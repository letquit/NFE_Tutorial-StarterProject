using Unity.Entities;
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
    /// </summary>
    public struct AbilityPrefabs : IComponentData
    {
        /// <summary>
        /// 范围伤害能力实体
        /// </summary>
        public Entity AoeAbility;
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
    /// </summary>
    public struct AbilityCooldownTicks : IComponentData
    {
        /// <summary>
        /// 范围伤害能力的冷却tick数
        /// </summary>
        public uint AoeAbility;
    }

    /// <summary>
    /// 预测能力冷却目标时间的命令数据组件
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AbilityCooldownTargetTicks : ICommandData
    {
        /// <summary>
        /// 当前命令的网络tick
        /// </summary>
        public NetworkTick Tick { get; set; }
        
        /// <summary>
        /// 范围伤害能力的网络tick
        /// </summary>
        public NetworkTick AoeAbility;
    }
}