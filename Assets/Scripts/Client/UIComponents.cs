using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 健康条UI引用组件，用于存储健康条UI游戏对象的引用
    /// 实现ICleanupComponentData接口以在实体清理时自动处理
    /// </summary>
    public class HealthBarUIReference : ICleanupComponentData
    {
        /// <summary>
        /// 存储健康条UI游戏对象的引用
        /// </summary>
        public GameObject Value;
    }

    /// <summary>
    /// 健康条偏移组件，用于存储健康条的3D位置偏移量
    /// </summary>
    public struct HealthBarOffset : IComponentData
    {
        /// <summary>
        /// 健康条的3D位置偏移量
        /// </summary>
        public float3 Value;
    }

    /// <summary>
    /// 技能射击UI引用组件，用于存储技能射击UI游戏对象的引用
    /// 实现ICleanupComponentData接口以在实体清理时自动处理
    /// </summary>
    public class SkillShotUIReference : ICleanupComponentData
    {
        /// <summary>
        /// 存储技能射击UI游戏对象的引用
        /// </summary>
        public GameObject Value;
    }
}