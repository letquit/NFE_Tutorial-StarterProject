using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 游戏开始属性的作者化组件，用于在Unity编辑器中配置游戏开始相关的参数
    /// </summary>
    public class GameStartPropertiesAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 每队最大玩家数量
        /// </summary>
        public int MaxPlayersPerTeam;
        
        /// <summary>
        /// 开始游戏所需的最小玩家数量
        /// </summary>
        public int MinPlayersToStartGame;
        
        /// <summary>
        /// 游戏开始倒计时时间（秒）
        /// </summary>
        public int CountdownTime;
        
        /// <summary>
        /// 玩家出生点偏移量数组
        /// </summary>
        public Vector3[] SpawnOffsets;

        /// <summary>
        /// 游戏开始属性烘焙器，将作者化组件转换为ECS实体组件
        /// </summary>
        public class GameStartPropertiesBaker : Baker<GameStartPropertiesAuthoring>
        {
            /// <summary>
            /// 将作者化组件的数据烘焙到ECS实体中
            /// </summary>
            /// <param name="authoring">游戏开始属性作者化组件实例</param>
            public override void Bake(GameStartPropertiesAuthoring authoring)
            {
                // 创建ECS实体
                var entity = GetEntity(TransformUsageFlags.None);
                
                // 添加游戏开始属性组件
                AddComponent(entity, new GameStartProperties
                {
                    MaxPlayersPerTeam = authoring.MaxPlayersPerTeam,
                    MinPlayersToStartGame = authoring.MinPlayersToStartGame,
                    CountdownTime = authoring.CountdownTime
                });
                
                // 添加队伍玩家计数器组件
                AddComponent<TeamPlayerCounter>(entity);

                // 添加出生点偏移缓冲区组件
                var spawnOffsets = AddBuffer<SpawnOffset>(entity);
                foreach (var spawnOffset in authoring.SpawnOffsets)
                {
                    spawnOffsets.Add(new SpawnOffset { Value = spawnOffset });
                }
            }
        }
    }
}
