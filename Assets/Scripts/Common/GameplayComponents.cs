using Common;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 标记游戏正在运行状态的组件数据结构
    /// </summary>
    public struct GamePlayingTag : IComponentData {}

    /// <summary>
    /// 存储游戏开始时间戳的组件数据结构
    /// </summary>
    public struct GameStartTick : IComponentData
    {
        /// <summary>
        /// 游戏开始时的网络时间戳
        /// </summary>
        public NetworkTick Value;
    }

    /// <summary>
    /// 标记游戏结束状态的组件数据结构
    /// </summary>
    public struct GameOverTag : IComponentData {}

    /// <summary>
    /// 存储获胜队伍信息的组件数据结构
    /// </summary>
    public struct WinningTeam : IComponentData
    {
        /// <summary>
        /// 获胜队伍类型，支持网络同步
        /// </summary>
        [GhostField] public TeamType Value;
    }
}