using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 初始化主摄像机系统，用于在客户端模拟环境中设置主摄像机实体组件
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class InitializeMainCameraSystem : SystemBase
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        protected override void OnCreate()
        {
            RequireForUpdate<MainCameraTag>();
        }

        /// <summary>
        /// 系统更新方法，负责将Unity主摄像机设置到对应的ECS实体组件中
        /// </summary>
        protected override void OnUpdate()
        {
            // 禁用当前系统以防止重复执行
            Enabled = false;
            
            // 获取带有MainCameraTag单例的实体
            var mainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            
            // 设置主摄像机组件数据
            EntityManager.SetComponentData(mainCameraEntity, new MainCamera { Value = Camera.main });
        }
    }
}