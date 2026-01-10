using System;
using Common;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 游戏结束界面控制器，负责管理游戏结束时的UI显示和玩家操作响应
    /// </summary>
    public class GameOverUIController : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private Button _returnToMainButton;
        [SerializeField] private Button _rageQuitButton;

        private EntityQuery _networkConnectionQuery;

        /// <summary>
        /// 当组件启用时调用，初始化事件监听器和网络连接查询
        /// </summary>
        private void OnEnable()
        {
            _returnToMainButton.onClick.AddListener(ReturnToMain);
            _rageQuitButton.onClick.AddListener(RageQuit);
            if (World.DefaultGameObjectInjectionWorld == null) return;
            // 创建网络流连接的实体查询
            _networkConnectionQuery =
                World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamConnection));
            var gameOverSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
            gameOverSystem.OnGameOver += ShowGameOverUI;
        }

        /// <summary>
        /// 处理返回主菜单的操作，断开网络连接并加载主场景
        /// </summary>
        private void ReturnToMain()
        {
            // 尝试获取网络连接实体并添加断开连接组件
            if (_networkConnectionQuery.TryGetSingletonEntity<NetworkStreamConnection>(out var networkConnectionEntity))
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<NetworkStreamRequestDisconnect>(
                    networkConnectionEntity);
            }
            
            // 销毁所有世界实例
            World.DisposeAllWorlds();
            
            // 加载第一个场景（主菜单）
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// 处理立即退出游戏的操作
        /// </summary>
        private void RageQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// 当组件禁用时调用，清理事件监听器
        /// </summary>
        private void OnDisable()
        {
            if (World.DefaultGameObjectInjectionWorld == null) return;
            var gameOverSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
            gameOverSystem.OnGameOver -= ShowGameOverUI;
        }

        /// <summary>
        /// 显示游戏结束UI界面
        /// </summary>
        /// <param name="winningTeam">获胜队伍类型</param>
        private void ShowGameOverUI(TeamType winningTeam)
        {
            _gameOverPanel.SetActive(true);
            _gameOverText.text = $"{winningTeam.ToString()} Team Win!";
        }
    }
}