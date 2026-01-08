using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// MinionSpawnAspect 提供了对小兵生成相关组件数据的访问和操作接口
    /// 实现了IAspect接口，用于在ECS系统中访问和操作小兵生成相关的组件数据
    /// </summary>
    public readonly partial struct MinionSpawnAspect : IAspect
    {
        private readonly RefRW<MinionSpawnTimers> _minionSpawnTimers;
        private readonly RefRO<MinionSpawnProperties> _minionSpawnProperties;

        /// <summary>
        /// 获取或设置当前波次中已生成的小兵数量
        /// </summary>
        public int CountSpawnedInWave
        {
            get => _minionSpawnTimers.ValueRO.CountSpawnedInWave;
            set => _minionSpawnTimers.ValueRW.CountSpawnedInWave = value;
        }

        /// <summary>
        /// 获取或设置到下一个小兵生成的时间
        /// </summary>
        private float TimeToNextMinion
        {
            get => _minionSpawnTimers.ValueRO.TimeToNextMinion;
            set => _minionSpawnTimers.ValueRW.TimeToNextMinion = value;
        }
        
        /// <summary>
        /// 获取或设置到下一次波次开始的时间
        /// </summary>
        private float TimeToNextWave
        {
            get => _minionSpawnTimers.ValueRO.TimeToNextWave;
            set => _minionSpawnTimers.ValueRW.TimeToNextWave = value;
        }

        /// <summary>
        /// 获取每波需要生成的小兵数量
        /// </summary>
        private int CountToSpawnInWave => _minionSpawnProperties.ValueRO.CountToSpawnInWave;

        /// <summary>
        /// 获取小兵之间的生成间隔时间
        /// </summary>
        private float TimeBetweenMinions => _minionSpawnProperties.ValueRO.TimeBetweenMinions;

        /// <summary>
        /// 获取波次之间的间隔时间
        /// </summary>
        private float TimeBetweenWaves => _minionSpawnProperties.ValueRO.TimeBetweenWaves;

        /// <summary>
        /// 判断是否应该生成小兵（波次时间和小兵时间都小于等于0）
        /// </summary>
        public bool ShouldSpawn => TimeToNextWave <= 0f && TimeToNextMinion <= 0f;

        /// <summary>
        /// 判断当前波次是否已经生成完毕（已生成数量大于等于目标数量）
        /// </summary>
        public bool IsWaveSpawned => CountSpawnedInWave >= CountToSpawnInWave;

        /// <summary>
        /// 根据给定的时间增量递减计时器
        /// </summary>
        /// <param name="deltaTime">时间增量，用于递减计时器</param>
        public void DecrementTimers(float deltaTime)
        {
            // 优先处理波次计时器，如果波次计时器未结束则递减波次计时器
            if (TimeToNextWave >= 0f)
            {
                TimeToNextWave -= deltaTime;
                return;
            }
            
            // 如果波次计时器已结束，则递减小兵计时器
            if (TimeToNextMinion >= 0f)
            {
                TimeToNextMinion -= deltaTime;
            }
        }

        /// <summary>
        /// 重置波次计时器为波次间隔时间
        /// </summary>
        public void ResetWaveTimer()
        {
            TimeToNextWave = TimeBetweenWaves;
        }

        /// <summary>
        /// 重置小兵计时器为小兵间隔时间
        /// </summary>
        public void ResetMinionTimer()
        {
            TimeToNextMinion = TimeBetweenMinions;
        }

        /// <summary>
        /// 重置小兵生成计数器为0
        /// </summary>
        public void ResetSpawnCounter()
        {
            CountSpawnedInWave = 0;
        }
    }
}