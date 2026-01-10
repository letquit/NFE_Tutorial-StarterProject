using Common;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 标记组件，用于标识需要重生的实体
    /// </summary>
    public struct RespawnEntityTag : IComponentData {}

    /// <summary>
    /// 重生缓冲区元素数据结构，存储实体重生相关信息
    /// </summary>
    public struct RespawnBufferElement : IBufferElementData
    {
        /// <summary>
        /// 重生时间点，使用网络节拍表示
        /// </summary>
        [GhostField] public NetworkTick RespawnTick;
        
        /// <summary>
        /// 关联的网络实体引用
        /// </summary>
        [GhostField] public Entity NetworkEntity;
        
        /// <summary>
        /// 网络ID，用于网络同步识别
        /// </summary>
        [GhostField] public int NetworkId;
    }

    /// <summary>
    /// 重生计数器组件，记录重生相关的tick值
    /// </summary>
    public struct RespawnTickCount : IComponentData
    {
        /// <summary>
        /// 当前重生计数值
        /// </summary>
        public uint Value;
    }

    /// <summary>
    /// 玩家生成信息组件，包含玩家队伍类型和生成位置
    /// </summary>
    public struct PlayerSpawnInfo : IComponentData
    {
        /// <summary>
        /// 玩家所属的队伍类型
        /// </summary>
        public TeamType MobaTeam;
        
        /// <summary>
        /// 玩家生成的位置坐标
        /// </summary>
        public float3 SpawnPosition;
    }

    /// <summary>
    /// 网络实体引用组件，用于存储对网络实体的引用
    /// </summary>
    public struct NetworkEntityReference : IComponentData
    {
        /// <summary>
        /// 实体引用值
        /// </summary>
        public Entity Value;
    }
}