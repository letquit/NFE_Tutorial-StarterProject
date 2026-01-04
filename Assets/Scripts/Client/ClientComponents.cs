using Common;
using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 客户端队伍请求组件数据结构
    /// 用于表示客户端请求加入的队伍类型
    /// </summary>
    public struct ClientTeamRequest : IComponentData
    {
        /// <summary>
        /// 请求的队伍类型值
        /// </summary>
        public TeamType Value;
    }
}