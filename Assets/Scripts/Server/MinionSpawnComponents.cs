using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 存储 小兵 生成属性的组件数据结构
    /// </summary>
    public struct MinionSpawnProperties : IComponentData
    {
        /// <summary>
        /// 波次之间的间隔时间
        /// </summary>
        public float TimeBetweenWaves;
        
        /// <summary>
        /// 每个 小兵 之间的生成间隔时间
        /// </summary>
        public float TimeBetweenMinions;
        
        /// <summary>
        /// 每波需要生成的 小兵 数量
        /// </summary>
        public int CountToSpawnInWave;
    }

    /// <summary>
    /// 存储 小兵 生成计时器的组件数据结构
    /// </summary>
    public struct MinionSpawnTimers : IComponentData
    {
        /// <summary>
        /// 到下一波生成的时间
        /// </summary>
        public float TimeToNextWave;
        
        /// <summary>
        /// 到下一个 小兵 生成的时间
        /// </summary>
        public float TimeToNextMinion;
        
        /// <summary>
        /// 当前波次中已生成的 小兵 数量
        /// </summary>
        public int CountSpawnedInWave;
    }

    /// <summary>
    /// 存储 小兵 路径容器的组件数据结构
    /// </summary>
    public struct MinionPathContainers : IComponentData
    {
        /// <summary>
        /// 顶部路径的实体引用
        /// </summary>
        public Entity TopLane;
        
        /// <summary>
        /// 中间路径的实体引用
        /// </summary>
        public Entity MidLane;
        
        /// <summary>
        /// 底部路径的实体引用
        /// </summary>
        public Entity BotLane;
    }
}