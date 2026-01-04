using Common;
using Unity.Entities;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    // 简单的标签组件，用于标识游戏中的英雄单位
    public struct ChampTag : IComponentData { }
    
    // 新英雄标签组件，用于标识新创建的英雄单位
    public struct NewChampTag : IComponentData { }
    
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
}