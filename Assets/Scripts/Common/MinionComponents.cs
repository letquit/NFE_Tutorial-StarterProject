using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 标记实体为 minion 的组件数据
    /// </summary>
    public struct MinionTag : IComponentData {}
    
    /// <summary>
    /// 标记新生成的 minion 的组件数据
    /// </summary>
    public struct NewMinionTag : IComponentData {}

    /// <summary>
    /// 存储 minion 路径位置的缓冲元素数据
    /// </summary>
    public struct MinionPathPosition : IBufferElementData
    {
        /// <summary>
        /// 路径位置的三维坐标值，使用 GhostField 进行网络同步，量化精度为 0
        /// </summary>
        [GhostField(Quantization = 0)] public float3 Value;
    }

    /// <summary>
    /// 存储 minion 当前路径索引的组件数据
    /// </summary>
    public struct MinionPathIndex : IComponentData
    {
        /// <summary>
        /// 当前路径索引值，使用 GhostField 进行网络同步
        /// </summary>
        [GhostField] public byte Value;
    }
}