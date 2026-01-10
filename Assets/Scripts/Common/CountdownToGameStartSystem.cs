using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 倒计时到游戏开始系统，负责处理游戏开始前的倒计时逻辑
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class CountdownToGameStartSystem : SystemBase
    {
        /// <summary>
        /// 当倒计时文本需要更新时触发的事件
        /// </summary>
        public Action<int> OnUpdateCountdownText;
        
        /// <summary>
        /// 当倒计时结束时触发的事件
        /// </summary>
        public Action OnCountdownEnd;

        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        protected override void OnCreate()
        {
            RequireForUpdate<NetworkTime>();
        }

        /// <summary>
        /// 系统更新主循环，处理倒计时逻辑和游戏状态转换
        /// </summary>
        protected override void OnUpdate()
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick) return;
            var currentTick = networkTime.ServerTick;
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // 遍历所有具有GameStartTick组件且带有Simulate标签的实体
            foreach (var (gameStartTick, entity) in SystemAPI.Query<GameStartTick>().WithAll<Simulate>().WithEntityAccess())
            {
                if (currentTick.Equals(gameStartTick.Value) || currentTick.IsNewerThan(gameStartTick.Value))
                {
                    // 创建游戏进行中的实体并销毁倒计时实体
                    var gamePlayingEntity = ecb.CreateEntity();
                    ecb.SetName(gamePlayingEntity, "GamePlayingEntity");
                    ecb.AddComponent<GamePlayingTag>(gamePlayingEntity);
                    
                    ecb.DestroyEntity(entity);
                    OnCountdownEnd?.Invoke();
                }
                else
                {
                    // 计算剩余时间并更新倒计时显示
                    var ticksToStart = gameStartTick.Value.TickIndexForValidTick - currentTick.TickIndexForValidTick;
                    var simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
                    var secondsToStart = (int)math.ceil((float)ticksToStart / simulationTickRate);
                    OnUpdateCountdownText?.Invoke(secondsToStart);
                }
            }
            
            ecb.Playback(EntityManager);
        }
    }
}