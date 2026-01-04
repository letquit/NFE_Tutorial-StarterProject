#if UNITY_EDITOR
using Unity.Entities;
using UnityEngine.SceneManagement;

namespace Helpers
{
    /// <summary>
    /// 加载连接场景系统，用于在编辑器模式下管理场景加载
    /// </summary>
    public partial class LoadConnectionSceneSystem : SystemBase
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <remarks>
        /// 检查当前活动场景是否为构建索引为0的场景，如果不是则加载该场景
        /// </remarks>
        protected override void OnCreate()
        {
            Enabled = false;
            // 检查当前活动场景是否为构建索引为0的场景，如果是则直接返回
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0)) return;
            // 加载构建索引为0的场景
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// 系统更新方法
        /// </summary>
        /// <remarks>
        /// 当前为空实现，系统在创建时已处理场景加载逻辑
        /// </remarks>
        protected override void OnUpdate()
        {
            
        }
    }
}

#endif