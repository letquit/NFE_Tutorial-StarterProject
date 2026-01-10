using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 游戏结束实体授权组件，用于在Unity编辑器中配置游戏结束相关的实体数据
    /// </summary>
    public class GameOverEntityAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 游戏结束实体烘焙器，负责将授权组件转换为ECS实体
        /// </summary>
        public class GameOverEntityBaker : Baker<GameOverEntityAuthoring>
        {
            /// <summary>
            /// 将授权组件烘焙为ECS实体
            /// </summary>
            /// <param name="authoring">游戏结束实体授权组件实例</param>
            public override void Bake(GameOverEntityAuthoring authoring)
            {
                // 创建实体并添加游戏结束标签组件和获胜队伍组件
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<GameOverTag>(entity);
                AddComponent<WinningTeam>(entity);
            }
        }
    }
}