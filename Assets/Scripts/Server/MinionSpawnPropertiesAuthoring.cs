using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 授权类，用于定义小兵生成的属性配置
    /// </summary>
    public class MinionSpawnPropertiesAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 波次之间的间隔时间
        /// </summary>
        public float TimeBetweenWaves;
        
        /// <summary>
        /// 小兵之间的生成间隔时间
        /// </summary>
        public float TimeBetweenMinions;
        
        /// <summary>
        /// 每波需要生成的小兵数量
        /// </summary>
        public int CountToSpawnInWave;

        /// <summary>
        /// 小兵生成属性的烘焙器类，负责将授权组件转换为实体组件
        /// </summary>
        public class MinionSpawnPropertiesBaker : Baker<MinionSpawnPropertiesAuthoring>
        {
            /// <summary>
            /// 将授权组件的数据烘焙为实体组件
            /// </summary>
            /// <param name="authoring">授权组件实例，包含小兵生成的配置数据</param>
            public override void Bake(MinionSpawnPropertiesAuthoring authoring)
            {
                // 获取当前对象对应的实体
                var entity = GetEntity(TransformUsageFlags.None);
                
                // 添加小兵生成属性组件
                AddComponent(entity, new MinionSpawnProperties
                {
                    TimeBetweenWaves = authoring.TimeBetweenWaves,
                    TimeBetweenMinions = authoring.TimeBetweenMinions,
                    CountToSpawnInWave = authoring.CountToSpawnInWave
                });
                
                // 添加小兵生成计时器组件，初始化计时器状态
                AddComponent(entity, new MinionSpawnTimers
                {
                    TimeToNextWave = authoring.TimeBetweenWaves,
                    TimeToNextMinion = 0f,
                    CountSpawnedInWave = 0
                });
            }
        }
    }
}