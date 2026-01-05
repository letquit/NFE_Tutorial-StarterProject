using Cinemachine;
using Common;
using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 相机控制器，负责处理游戏中的相机移动、缩放和定位功能
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
        
        [Header("Move Settings")]
        [SerializeField] private bool _drawBounds;
        [SerializeField] private Bounds _cameraBounds;
        [SerializeField] private float _camSpeed;
        [SerializeField] private Vector2 _screenPercentageDetection;

        [Header("Zoom Settings")]
        [SerializeField] private float _minZoomDistance;
        [SerializeField] private float _maxZoomDistance;
        [SerializeField] private float _zoomSpeed;

        [Header("Camera Start Positions")] 
        [SerializeField] private Vector3 _redTeamPosition = new(50f, 0f, 50f);
        [SerializeField] private Vector3 _blueTeamPosition = new(-50f, 0f, -50f);
        [SerializeField] private Vector3 _spectatorPosition = new(0f, 0f, 0f);
        
        private Vector2 _normalScreenPercentage;
        
        /// <summary>
        /// 获取标准化鼠标位置，范围在0到1之间
        /// </summary>
        private Vector2 NormalMousePos => new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        
        /// <summary>
        /// 检查鼠标是否在屏幕左侧边缘区域
        /// </summary>
        private bool InScreenLeft => NormalMousePos.x < _normalScreenPercentage.x  && Application.isFocused;
        
        /// <summary>
        /// 检查鼠标是否在屏幕右侧边缘区域
        /// </summary>
        private bool InScreenRight => NormalMousePos.x > 1 - _normalScreenPercentage.x  && Application.isFocused;
        
        /// <summary>
        /// 检查鼠标是否在屏幕顶部边缘区域
        /// </summary>
        private bool InScreenTop => NormalMousePos.y < _normalScreenPercentage.y  && Application.isFocused;
        
        /// <summary>
        /// 检查鼠标是否在屏幕底部边缘区域
        /// </summary>
        private bool InScreenBottom => NormalMousePos.y > 1 - _normalScreenPercentage.y  && Application.isFocused;

        private CinemachineFramingTransposer _transposer;
        private EntityManager _entityManager;
        private EntityQuery _teamControllerQuery;
        private EntityQuery _localChampQuery;
        private bool _cameraSet;
        
        /// <summary>
        /// 初始化相机组件和标准化屏幕百分比
        /// </summary>
        private void Awake()
        {
            _normalScreenPercentage = _screenPercentageDetection * 0.01f;
            _transposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        /// <summary>
        /// 初始化实体管理器和查询，并根据玩家团队设置相机初始位置
        /// </summary>
        private void Start()
        {
            if (World.DefaultGameObjectInjectionWorld == null) return;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _teamControllerQuery = _entityManager.CreateEntityQuery(typeof(ClientTeamRequest));
            _localChampQuery = _entityManager.CreateEntityQuery(typeof(OwnerChampTag));

            // Move the camera to the base corresponding to the team the player is on.
            // Spectators' cameras will start in the center of the map
            if (_teamControllerQuery.TryGetSingleton<ClientTeamRequest>(out var requestedTeam))
            {
                var team = requestedTeam.Value;
                var cameraPosition = team switch
                {
                    TeamType.Blue => _blueTeamPosition,
                    TeamType.Red => _redTeamPosition,
                    _ => _spectatorPosition
                };
                transform.position = cameraPosition;

                if (team != TeamType.AutoAssign)
                {
                    _cameraSet = true;
                }
            }
        }

        /// <summary>
        /// 在编辑器中验证时更新标准化屏幕百分比
        /// </summary>
        private void OnValidate()
        {
            _normalScreenPercentage = _screenPercentageDetection * 0.01f;
        }

        /// <summary>
        /// 更新相机状态，包括自动分配团队相机设置、相机移动和缩放
        /// </summary>
        private void Update()
        {
            SetCameraForAutoAssignTeam();
            MoveCamera();
            ZoomCamera();
        }

        /// <summary>
        /// 根据鼠标位置在屏幕边缘的检测来移动相机
        /// 并确保相机位置在指定边界范围内
        /// </summary>
        private void MoveCamera()
        {
            if (InScreenLeft)
            {
                transform.position += Vector3.left * (_camSpeed * Time.deltaTime);
            }

            if (InScreenRight)
            {
                transform.position += Vector3.right * (_camSpeed * Time.deltaTime);
            }

            if (InScreenTop)
            {
                transform.position += Vector3.back * (_camSpeed * Time.deltaTime);
            }

            if (InScreenBottom)
            {
                transform.position += Vector3.forward * (_camSpeed * Time.deltaTime);
            }
            
            if (!_cameraBounds.Contains(transform.position))
            {
                transform.position = _cameraBounds.ClosestPoint(transform.position);
            }
        }

        /// <summary>
        /// 处理鼠标滚轮输入来缩放相机距离
        /// </summary>
        private void ZoomCamera()
        {
            if (Mathf.Abs(Input.mouseScrollDelta.y) > float.Epsilon)
            {
                _transposer.m_CameraDistance -= Input.mouseScrollDelta.y * _zoomSpeed * Time.deltaTime;
                _transposer.m_CameraDistance =
                    Mathf.Clamp(_transposer.m_CameraDistance, _minZoomDistance, _maxZoomDistance);
            }
        }

        /// <summary>
        /// 为自动分配团队的玩家设置相机位置
        /// </summary>
        private void SetCameraForAutoAssignTeam()
        {
            if (!_cameraSet)
            {
                if (_localChampQuery.TryGetSingletonEntity<OwnerChampTag>(out var localChamp))
                {
                    var team = _entityManager.GetComponentData<MobaTeam>(localChamp).Value;
                    var cameraPosition = team switch
                    {
                        TeamType.Blue => _blueTeamPosition,
                        TeamType.Red => _redTeamPosition,
                        _ => _spectatorPosition
                    };
                    transform.position = cameraPosition;
                    _cameraSet = true;
                }
            }
        }

        /// <summary>
        /// 在编辑器中绘制相机边界线框
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_drawBounds) return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_cameraBounds.center, _cameraBounds.size);
        }
    }
}