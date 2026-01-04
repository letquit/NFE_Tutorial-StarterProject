using Common;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 客户端连接管理器，负责处理网络连接相关的UI交互和连接逻辑
    /// </summary>
    public class ClientConnectionManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _addressField;
        [SerializeField] private TMP_InputField _portField;
        [SerializeField] private TMP_Dropdown _connectionModeDropdown;
        [SerializeField] private TMP_Dropdown _teamDropdown;
        [SerializeField] private Button _connectButton;
        
        /// <summary>
        /// 获取端口号，从端口输入字段解析
        /// </summary>
        private ushort Port => ushort.Parse(_portField.text);
        
        /// <summary>
        /// 获取地址，从地址输入字段获取
        /// </summary>
        private string Address => _addressField.text;

        /// <summary>
        /// 当组件启用时调用，注册UI事件监听器
        /// </summary>
        private void OnEnable()
        {
            _connectionModeDropdown.onValueChanged.AddListener(OnConnectionModeChanged);
            _connectButton.onClick.AddListener(OnButtonConnect);
            OnConnectionModeChanged(_connectionModeDropdown.value);
        }

        /// <summary>
        /// 当组件禁用时调用，移除UI事件监听器
        /// </summary>
        private void OnDisable()
        {
            _connectionModeDropdown.onValueChanged.RemoveAllListeners();
            _connectButton.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 连接模式改变时的回调函数，更新连接按钮的标签文本
        /// </summary>
        /// <param name="connectionMode">连接模式索引值</param>
        private void OnConnectionModeChanged(int connectionMode)
        {
            string buttonLabel;
            _connectButton.enabled = true;
            
            switch (connectionMode)
            {
                case 0:
                    buttonLabel = "Start Host";
                    break;
                case 1 : 
                    buttonLabel = "Start Server";
                    break;
                case 2:
                    buttonLabel = "Start Client";
                    break;
                default:
                    buttonLabel = "<ERROR>";
                    _connectButton.enabled = false;
                    break;
            }

            var buttonText = _connectButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = buttonLabel;
        }

        /// <summary>
        /// 连接按钮点击时的回调函数，处理不同连接模式的启动逻辑
        /// </summary>
        private void OnButtonConnect()
        {
            DestroyLocalSimulationWorld();
            SceneManager.LoadScene(1);
            
            switch (_connectionModeDropdown.value)
            {
                case 0:
                    StartServer();
                    StartClient();
                    break;
                case 1:
                    StartServer();
                    break;
                case 2:
                    StartClient();
                    break;
                default:
                    Debug.LogError("Error: Unknown connection mode", gameObject);
                    break;
            }
        }

        /// <summary>
        /// 销毁本地模拟世界，清理现有的游戏世界实例
        /// </summary>
        private static void DestroyLocalSimulationWorld()
        {
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }
        }

        /// <summary>
        /// 启动服务器，创建服务器世界并开始监听指定端口
        /// </summary>
        private void StartServer()
        {
            var serverWorld = ClientServerBootstrap.CreateServerWorld("Turbo Server World");

            var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);
            {
                using var networkDriverQuery =
                    serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
            }
        }

        /// <summary>
        /// 启动客户端，创建客户端世界并连接到指定服务器
        /// </summary>
        private void StartClient()
        {
            // 创建客户端世界
            var clientWorld = ClientServerBootstrap.CreateClientWorld("Turbo Client World");

            // 解析连接端点
            var connectionEndpoint = NetworkEndpoint.Parse(Address, Port);
            {
                // 获取网络驱动查询并连接到服务器
                using var networkDriverQuery =
                    clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW
                    .Connect(clientWorld.EntityManager, connectionEndpoint);
            }

            // 根据下拉菜单值映射队伍类型
            var team = _teamDropdown.value switch
            {
                0 => TeamType.AutoAssign,
                1 => TeamType.Blue,
                2 => TeamType.Red,
                _ => TeamType.None
            };
            
            // 创建队伍请求实体
            var teamRequestEntity = clientWorld.EntityManager.CreateEntity();
            clientWorld.EntityManager.AddComponentData(teamRequestEntity, new ClientTeamRequest
            {
                Value = team
            });
        }
    }
}