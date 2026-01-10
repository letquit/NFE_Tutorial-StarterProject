using System;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 客户端游戏开始系统，负责处理客户端的游戏开始相关逻辑
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class ClientStartGameSystem : SystemBase
    {
        /// <summary>
        /// 当玩家剩余数量更新时触发的事件
        /// </summary>
        public Action<int> OnUpdatePlayersRemainingToStart;
        
        /// <summary>
        /// 当游戏开始倒计时时触发的事件
        /// </summary>
        public Action OnStartGameCountdown;
        
        /// <summary>
        /// 更新系统的主要方法，处理游戏开始相关的RPC命令
        /// </summary>
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // 处理玩家剩余数量更新的RPC命令
            foreach (var (playersRemainingToStart, entity) in SystemAPI.Query<PlayersRemainingToStart>()
                         .WithAll<ReceiveRpcCommandRequest>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                OnUpdatePlayersRemainingToStart?.Invoke(playersRemainingToStart.Value);
            }
            
            // 处理游戏开始倒计时的RPC命令
            foreach (var (gameStartTick, entity) in SystemAPI.Query<GameStartTickRpc>().WithAll<ReceiveRpcCommandRequest>()
                         .WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                OnStartGameCountdown?.Invoke();

                var gameStartEntity = ecb.CreateEntity();
                ecb.AddComponent(gameStartEntity, new GameStartTick
                {
                    Value = gameStartTick.Value
                });
            }
            
            ecb.Playback(EntityManager);
        }
    }
}