using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 血条系统，负责管理游戏实体的血条显示，包括血条的创建、更新和销毁
    /// </summary>
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBarSystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<UIPrefabs>();
        }

        /// <summary>
        /// 系统更新主方法，处理血条的创建、更新和清理
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // 为需要血条的实体生成生命条
            foreach (var (transform, healthBarOffset, maxHitPoints, entity) in SystemAPI
                         .Query<LocalTransform, HealthBarOffset, MaxHitPoints>().WithNone<HealthBarUIReference>()
                         .WithEntityAccess())
            {
                var healthBarPrefab = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().HealthBar;
                var spawnPosition = transform.Position + healthBarOffset.Value;
                var newHealthBar = Object.Instantiate(healthBarPrefab, spawnPosition, Quaternion.identity);
                SetHealthBar(newHealthBar, maxHitPoints.Value, maxHitPoints.Value);
                ecb.AddComponent(entity, new HealthBarUIReference { Value = newHealthBar });
            }
            
            // 更新血条的位置和数值
            foreach (var (transform, healthBarOffset, currentHitPoints, maxHitPoints, healthBarUI) in SystemAPI
                         .Query<LocalTransform, HealthBarOffset, CurrentHitPoints, MaxHitPoints,
                             HealthBarUIReference>())
            {
                var healthBarPosition = transform.Position + healthBarOffset.Value;
                healthBarUI.Value.transform.position = healthBarPosition;
                SetHealthBar(healthBarUI.Value, currentHitPoints.Value, maxHitPoints.Value);
            }
            
            // 关联实体被摧毁后，清理血条
            foreach (var (healthBarUI, entity) in SystemAPI.Query<HealthBarUIReference>().WithNone<LocalTransform>()
                         .WithEntityAccess())
            {
                Object.Destroy(healthBarUI.Value);
                ecb.RemoveComponent<HealthBarUIReference>(entity);
            }
        }

        /// <summary>
        /// 设置血条的数值显示
        /// </summary>
        /// <param name="healthBarCanvasObject">血条游戏对象</param>
        /// <param name="curHitPoints">当前生命值</param>
        /// <param name="maxHitPoints">最大生命值</param>
        private void SetHealthBar(GameObject healthBarCanvasObject, int curHitPoints, int maxHitPoints)
        {
            var healthBarSlider = healthBarCanvasObject.GetComponentInChildren<Slider>();
            healthBarSlider.minValue = 0;
            healthBarSlider.maxValue = maxHitPoints;
            healthBarSlider.value = curHitPoints;
        }
    }
}