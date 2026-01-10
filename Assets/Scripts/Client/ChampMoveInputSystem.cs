using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 角色移动输入系统，处理玩家的移动位置选择输入
    /// </summary>
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class ChampMoveInputSystem : SystemBase
    {
        private MobaInputActions _inputActions;
        private CollisionFilter _selectionFilter;

        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        protected override void OnCreate()
        {
            RequireForUpdate<GamePlayingTag>();
            _inputActions = new MobaInputActions();
            _selectionFilter = new CollisionFilter
            {
                BelongsTo = 1 << 5, // Raycasts
                CollidesWith = 1 << 0 // GroundPlane
            };
            RequireForUpdate<OwnerChampTag>();
        }

        /// <summary>
        /// 系统开始运行时的初始化方法
        /// </summary>
        protected override void OnStartRunning()
        {
            _inputActions.Enable();
            _inputActions.GameplayMap.SelectMovePosition.performed += OnSelectMovePosition;
        }
        
        /// <summary>
        /// 系统停止运行时的清理方法
        /// </summary>
        protected override void OnStopRunning()
        {
            _inputActions.GameplayMap.SelectMovePosition.performed -= OnSelectMovePosition;
            _inputActions.Disable();
        }

        /// <summary>
        /// 处理选择移动位置的输入回调
        /// </summary>
        /// <param name="obj">输入动作回调上下文</param>
        private void OnSelectMovePosition(InputAction.CallbackContext obj)
        {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            var mainCamera = EntityManager.GetComponentObject<MainCamera>(cameraEntity).Value;

            var mousePosition = Input.mousePosition;
            mousePosition.z = 100f;
            var worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            // 构建射线检测输入参数
            var selectionInput = new RaycastInput
            {
                Start = mainCamera.transform.position,
                End = worldPosition,
                Filter = _selectionFilter
            };

            if (collisionWorld.CastRay(selectionInput, out var closestHit))
            {
                var champEntity = SystemAPI.GetSingletonEntity<OwnerChampTag>();
                EntityManager.SetComponentData(champEntity, new ChampMoveTargetPosition
                {
                    Value = closestHit.Position
                });
            }
        }


        /// <summary>
        /// 系统更新方法
        /// </summary>
        protected override void OnUpdate()
        { 
        }
    }
}