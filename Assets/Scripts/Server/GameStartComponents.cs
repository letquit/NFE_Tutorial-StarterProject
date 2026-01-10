using Unity.Entities;
using Unity.Mathematics;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 游戏开始属性组件数据结构，用于存储游戏启动时的相关配置参数
    /// </summary>
    public struct GameStartProperties : IComponentData
    {
        /// <summary>
        /// 每队最大玩家数量
        /// </summary>
        public int MaxPlayersPerTeam;
        
        /// <summary>
        /// 开始游戏所需的最少玩家数量
        /// </summary>
        public int MinPlayersToStartGame;
        
        /// <summary>
        /// 游戏开始前的倒计时时间（秒）
        /// </summary>
        public int CountdownTime;
    }

    /// <summary>
    /// 队伍玩家计数器组件数据结构，用于跟踪各队伍的玩家数量
    /// </summary>
    public struct TeamPlayerCounter : IComponentData
    {
        /// <summary>
        /// 蓝队玩家数量
        /// </summary>
        public int BlueTeamPlayers;
        
        /// <summary>
        /// 红队玩家数量
        /// </summary>
        public int RedTeamPlayers;
        
        /// <summary>
        /// 获取总玩家数量（蓝队和红队玩家数量之和）
        /// </summary>
        public int TotalPlayers => BlueTeamPlayers + RedTeamPlayers;
    }

    /// <summary>
    /// 生成偏移缓冲元素数据结构，用于定义实体生成位置的偏移量
    /// </summary>
    public struct SpawnOffset : IBufferElementData
    {
        /// <summary>
        /// 三维浮点向量形式的位置偏移值
        /// </summary>
        public float3 Value;
    }
}