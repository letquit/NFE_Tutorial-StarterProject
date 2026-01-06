using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 瞄准技能射击系统，处理玩家瞄准输入并计算瞄准方向
    /// </summary>
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct AimSkillShotSystem : ISystem
    {
        private CollisionFilter _selectionFilter;

        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainCameraTag>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            // 设置碰撞过滤器，定义属于第5层，与第0层发生碰撞
            _selectionFilter = new CollisionFilter
            {
                BelongsTo = 1 << 5,
                CollidesWith = 1 << 0
            };
        }

        /// <summary>
        /// 系统更新方法，处理瞄准输入逻辑
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            // 遍历所有具有瞄准输入、本地变换组件且带有瞄准技能射击标签和所有者标签的实体
            foreach (var (aimInput, transform) in SystemAPI.Query<RefRW<AimInput>, LocalTransform>()
                         .WithAll<AimSkillShotTag, OwnerChampTag>())
            {
                // 获取物理世界单例的碰撞世界
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                // 获取主相机实体
                var cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
                // 获取主相机组件对象
                var mainCamera = state.EntityManager.GetComponentObject<MainCamera>(cameraEntity).Value;

                // 获取鼠标屏幕位置并设置Z轴深度
                var mousePosition = Input.mousePosition;
                mousePosition.z = 1000f;
                // 将屏幕坐标转换为世界坐标
                var worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

                // 创建射线检测输入，用于检测鼠标指向的位置
                var selectionInput = new RaycastInput
                {
                    Start = mainCamera.transform.position,
                    End = worldPosition,
                    Filter = _selectionFilter
                };

                // 执行射线检测，如果击中目标则计算瞄准方向
                if (collisionWorld.CastRay(selectionInput, out var closestHit))
                {
                    // 计算到目标的方向向量，保持Y轴高度不变
                    var directionToTarget = closestHit.Position - transform.Position;
                    directionToTarget.y = transform.Position.y;
                    directionToTarget = math.normalize(directionToTarget);
                    // 设置瞄准输入的值为计算出的方向
                    aimInput.ValueRW.Value = directionToTarget;
                }
            }
        }
    }
}