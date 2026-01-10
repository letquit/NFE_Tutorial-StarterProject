using System;
using Common;
using Unity.Entities;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 游戏结束系统，负责处理游戏结束逻辑和清理工作
    /// </summary>
    public partial class GameOverSystem : SystemBase
    {
        /// <summary>
        /// 游戏结束事件回调，当游戏结束时触发
        /// </summary>
        public Action<TeamType> OnGameOver;

        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        protected override void OnCreate()
        {
            // 要求更新时必须存在GameOverTag单例组件
            RequireForUpdate<GameOverTag>();
            // 要求更新时必须存在GamePlayingTag单例组件
            RequireForUpdate<GamePlayingTag>();
        }

        /// <summary>
        /// 系统更新方法，执行游戏结束逻辑
        /// </summary>
        protected override void OnUpdate()
        {
            // 获取游戏结束实体并获取获胜队伍信息
            var gameOverEntity = SystemAPI.GetSingletonEntity<GameOverTag>();
            var winningTeam = SystemAPI.GetComponent<WinningTeam>(gameOverEntity).Value;
            // 触发游戏结束事件回调
            OnGameOver?.Invoke(winningTeam);

            // 获取游戏进行中实体并销毁它
            var gamePlayingEntity = SystemAPI.GetSingletonEntity<GamePlayingTag>();
            EntityManager.DestroyEntity(gamePlayingEntity);

            // 禁用当前系统以防止重复执行
            Enabled = false;
        }
    }
}