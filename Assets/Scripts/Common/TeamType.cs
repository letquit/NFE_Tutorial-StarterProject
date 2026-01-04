using UnityEngine;

namespace Common
{
    /// <summary>
    /// 表示团队类型的枚举
    /// </summary>
    public enum TeamType : byte
    {
        /// <summary>
        /// 无团队
        /// </summary>
        None = 0,
        
        /// <summary>
        /// 蓝队
        /// </summary>
        Blue = 1,
        
        /// <summary>
        /// 红队
        /// </summary>
        Red = 2,
        
        /// <summary>
        /// 自动分配团队
        /// </summary>
        AutoAssign = byte.MaxValue
    }
}