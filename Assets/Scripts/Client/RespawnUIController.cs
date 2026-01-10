using System;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 负责管理重生界面显示的MonoBehaviour组件
    /// </summary>
    public class RespawnUIController : MonoBehaviour
    {
        [SerializeField] private GameObject _respawnPanel;
        [SerializeField] private TextMeshProUGUI _respawnCountdownText;

        /// <summary>
        /// 当组件启用时调用，初始化重生系统事件监听
        /// </summary>
        private void OnEnable()
        {
            // 隐藏重生面板
            _respawnPanel.SetActive(false);
            
            // 检查默认游戏对象注入世界是否存在
            if (World.DefaultGameObjectInjectionWorld == null) return;
            var respawnSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RespawnChampSystem>();
            if (respawnSystem != null)
            {
                // 订阅重生系统的事件
                respawnSystem.OnUpdateRespawnCountdown += UpdateRespawnCountdownText;
                respawnSystem.OnRespawn += CloseRespawnPanel;
            }
        }

        /// <summary>
        /// 当组件禁用时调用，清理重生系统事件监听
        /// </summary>
        private void OnDisable()
        {
            // 检查默认游戏对象注入世界是否存在
            if (World.DefaultGameObjectInjectionWorld == null) return;
            var respawnSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RespawnChampSystem>();
            if (respawnSystem != null)
            {
                // 取消订阅重生系统的事件
                respawnSystem.OnUpdateRespawnCountdown -= UpdateRespawnCountdownText;
                respawnSystem.OnRespawn -= CloseRespawnPanel;
            }
        }

        /// <summary>
        /// 更新重生倒计时文本显示
        /// </summary>
        /// <param name="countdownTime">倒计时时间</param>
        private void UpdateRespawnCountdownText(int countdownTime)
        {
            if (!_respawnPanel.activeSelf) _respawnPanel.SetActive(true);
            
            _respawnCountdownText.text = countdownTime.ToString();
        }

        /// <summary>
        /// 关闭重生面板
        /// </summary>
        private void CloseRespawnPanel()
        {
            _respawnPanel.SetActive(false);
        }
    }
}