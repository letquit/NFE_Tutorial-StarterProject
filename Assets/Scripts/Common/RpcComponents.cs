using Common;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// Moba团队请求结构体，用于在网络中传输团队类型信息
    /// 实现IRpcCommand接口以支持远程过程调用
    /// </summary>
    public struct MobaTeamRequest : IRpcCommand
    {
        /// <summary>
        /// 团队类型值，表示请求的团队信息
        /// </summary>
        public TeamType Value;
    }

    /// <summary>
    /// 玩家剩余数量到开始结构体，用于传输游戏中剩余玩家数量信息
    /// 实现IRpcCommand接口以支持远程过程调用
    /// </summary>
    public struct PlayersRemainingToStart : IRpcCommand
    {
        /// <summary>
        /// 剩余玩家数量值
        /// </summary>
        public int Value;
    }

    /// <summary>
    /// 游戏开始时间戳RPC结构体，用于传输游戏开始的网络时间戳
    /// 实现IRpcCommand接口以支持远程过程调用
    /// </summary>
    public struct GameStartTickRpc : IRpcCommand
    {
        /// <summary>
        /// 游戏开始的网络时间戳值
        /// </summary>
        public NetworkTick Value;
    }
}