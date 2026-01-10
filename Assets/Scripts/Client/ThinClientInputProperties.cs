using Unity.Entities;
using Unity.Mathematics;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 表示瘦客户端输入属性的数据组件，用于存储随机数生成器、计时器设置和位置范围信息
    /// </summary>
    public struct ThinClientInputProperties : IComponentData
    {
        /// <summary>
        /// 随机数生成器，用于生成随机数值
        /// </summary>
        public Random Random;
        
        /// <summary>
        /// 当前计时器值
        /// </summary>
        public float Timer;
        
        /// <summary>
        /// 计时器最小值限制
        /// </summary>
        public float MinTimer;
        
        /// <summary>
        /// 计时器最大值限制
        /// </summary>
        public float MaxTimer;
        
        /// <summary>
        /// 位置范围的最小坐标值
        /// </summary>
        public float3 MinPosition;
        
        /// <summary>
        /// 位置范围的最大坐标值
        /// </summary>
        public float3 MaxPosition;
    }
}