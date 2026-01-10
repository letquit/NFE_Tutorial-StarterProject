using System.Collections;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 游戏开始界面控制器，负责管理游戏开始前的UI界面交互，包括等待玩家加入、退出确认、倒计时等功能
    /// </summary>
    public class GameStartUIController : MonoBehaviour
    {
        [SerializeField] private GameObject _beginGamePanel;
        [SerializeField] private GameObject _confirmQuitPanel;
        [SerializeField] private GameObject _countdownPanel;
        
        [SerializeField] private Button _quitWaitingButton;
        [SerializeField] private Button _confirmQuitButton;
        [SerializeField] private Button _cancelQuitButton;
        
        [SerializeField] private TextMeshProUGUI _waitingText;
        [SerializeField] private TextMeshProUGUI _countdownText;
        
        private EntityQuery _networkConnectionQuery;
        private EntityManager _entityManager;
        
        /// <summary>
        /// 当组件启用时调用，初始化UI面板状态、按钮事件监听器、实体管理系统以及系统事件订阅
        /// </summary>
        private void OnEnable()
        {
            _beginGamePanel.SetActive(true);
            
            _quitWaitingButton.onClick.AddListener(AttemptQuitWaiting);
            _confirmQuitButton.onClick.AddListener(ConfirmQuit);
            _cancelQuitButton.onClick.AddListener(CancelQuit);

            // 初始化实体管理系统和网络连接查询
            if (World.DefaultGameObjectInjectionWorld == null) return;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _networkConnectionQuery = _entityManager.CreateEntityQuery(typeof(NetworkStreamConnection));
			
            // 订阅客户端开始游戏系统的事件
            var startGameSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ClientStartGameSystem>();
            if (startGameSystem != null)
            {
                startGameSystem.OnUpdatePlayersRemainingToStart += UpdatePlayerRemainingText;
                startGameSystem.OnStartGameCountdown += BeginCountdown;
            }

            // 订阅倒计时系统的事件
            var countdownSystem = World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<CountdownToGameStartSystem>();
            if (countdownSystem != null)
            {
                countdownSystem.OnUpdateCountdownText += UpdateCountdownText;
                countdownSystem.OnCountdownEnd += EndCountdown;
            }
        }
        
        /// <summary>
        /// 当组件禁用时调用，清理按钮事件监听器和系统事件订阅
        /// </summary>
        private void OnDisable()
        {
            _quitWaitingButton.onClick.RemoveAllListeners();
            _confirmQuitButton.onClick.RemoveAllListeners();
            _cancelQuitButton.onClick.RemoveAllListeners();
            
            if (World.DefaultGameObjectInjectionWorld == null) return;
            // 取消订阅客户端开始游戏系统的事件
            var startGameSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ClientStartGameSystem>();
            if (startGameSystem != null)
            {
                startGameSystem.OnUpdatePlayersRemainingToStart -= UpdatePlayerRemainingText;
                startGameSystem.OnStartGameCountdown -= BeginCountdown;
            }
            
            // 取消订阅倒计时系统的事件
            var countdownSystem = World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<CountdownToGameStartSystem>();
            if (countdownSystem != null)
            {
                countdownSystem.OnUpdateCountdownText -= UpdateCountdownText;
                countdownSystem.OnCountdownEnd -= EndCountdown;
            }
        }
        
        /// <summary>
        /// 更新剩余玩家数量显示文本
        /// </summary>
        /// <param name="playersRemainingToStart">剩余需要加入的玩家数量</param>
        private void UpdatePlayerRemainingText(int playersRemainingToStart)
        {
            var playersText = playersRemainingToStart == 1 ? "player" : "players";
            _waitingText.text = $"Waiting for {playersRemainingToStart.ToString()} more {playersText} to join...";
        }

        /// <summary>
        /// 更新倒计时显示文本
        /// </summary>
        /// <param name="countdownTime">当前倒计时时间</param>
        private void UpdateCountdownText(int countdownTime)
        {
            _countdownText.text = countdownTime.ToString();
        }
        
        /// <summary>
        /// 尝试退出等待状态，显示退出确认面板
        /// </summary>
        private void AttemptQuitWaiting()
        {
            _beginGamePanel.SetActive(false);
            _confirmQuitPanel.SetActive(true);
        }

        /// <summary>
        /// 确认退出游戏，启动断开连接协程
        /// </summary>
        private void ConfirmQuit()
        {
            StartCoroutine(DisconnectDelay());
        }

        /// <summary>
        /// 延迟断开网络连接并返回主菜单的协程
        /// </summary>
        /// <returns>等待1秒后执行断开连接操作</returns>
        IEnumerator DisconnectDelay()
        {
            yield return new WaitForSeconds(1f);
            // 获取网络连接实体并添加断开连接请求组件
            if (_networkConnectionQuery.TryGetSingletonEntity<NetworkStreamConnection>(out var networkConnectionEntity))
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<NetworkStreamRequestDisconnect>(
                    networkConnectionEntity);
            }
            World.DisposeAllWorlds();
            SceneManager.LoadScene(0);
        }
        
        /// <summary>
        /// 取消退出操作，返回开始游戏面板
        /// </summary>
        private void CancelQuit()
        {
            _confirmQuitPanel.SetActive(false);
            _beginGamePanel.SetActive(true);
        }

        /// <summary>
        /// 开始倒计时，隐藏其他面板并显示倒计时面板
        /// </summary>
        private void BeginCountdown()
        {
            _beginGamePanel.SetActive(false);
            _confirmQuitPanel.SetActive(false);
            _countdownPanel.SetActive(true);
        }

        /// <summary>
        /// 结束倒计时，隐藏倒计时面板
        /// </summary>
        private void EndCountdown()
        {
            _countdownPanel.SetActive(false);
        }
    }
}