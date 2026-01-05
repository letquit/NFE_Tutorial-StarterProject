using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 组件数据，用于存储主摄像机的引用
    /// </summary>
    public class MainCamera : IComponentData
    {
        /// <summary>
        /// 存储Unity Camera组件的引用
        /// </summary>
        public Camera Value;
    }
    
    /// <summary>
    /// 标记组件，用于标识主摄像机实体
    /// </summary>
    public struct MainCameraTag : IComponentData {}
}