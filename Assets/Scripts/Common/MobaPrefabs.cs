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

        /// <summary>
        /// 小兵的实体预制体引用
        /// </summary>
        public Entity Minion;

        /// <summary>
        /// 游戏结束界面的实体预制体引用
        /// </summary>
        public Entity GameOverEntity;

        /// <summary>
        /// 复活相关的实体预制体引用
        /// </summary>
        public Entity RespawnEntity;
    }

    /// <summary>
    /// 存储UI预制体的组件数据类
    /// 用于管理游戏中UI相关的预制体对象引用
    /// </summary>
    public class UIPrefabs : IComponentData
    {
        /// <summary>
        /// 血条UI预制体对象
        /// </summary>
        public GameObject HealthBar;
        
        /// <summary>
        /// 技能射击UI预制体对象
        /// </summary>
        public GameObject SkillShot;
    }
}