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
}