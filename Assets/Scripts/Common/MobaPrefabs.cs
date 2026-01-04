using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 存储MOBA游戏相关预制体的组件数据结构
    /// 用于在ECS系统中管理游戏对象的预制体引用
    /// </summary>
    public struct MobaPrefabs : IComponentData
    {
        /// <summary>
        /// 英雄角色的实体预制体引用
        /// </summary>
        public Entity Champion;
    }
}