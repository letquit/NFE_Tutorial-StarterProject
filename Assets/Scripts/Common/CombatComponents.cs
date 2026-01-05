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
}