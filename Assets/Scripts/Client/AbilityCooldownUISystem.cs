using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 负责处理技能冷却UI系统的更新，仅在客户端模拟世界中运行
    /// 该系统根据网络时间更新技能冷却遮罩的显示
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct AbilityCooldownUISystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        /// <summary>
        /// 系统更新方法，处理技能冷却UI的更新逻辑
        /// 遍历所有具有技能冷却目标tick和技能冷却tick组件的实体，
        /// 根据当前服务器tick计算剩余冷却时间并更新UI遮罩
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var abilityCooldownUIController = AbilityCooldownUIController.Instance;

            foreach (var (cooldownTargetTicks, abilityCooldownTicks) in SystemAPI
                         .Query<DynamicBuffer<AbilityCooldownTargetTicks>, AbilityCooldownTicks>())
            {
                // 获取当前tick的技能冷却目标tick数据，如果不存在则初始化为无效值
                if (!cooldownTargetTicks.GetDataAtTick(currentTick, out var curTargetTicks))
                {
                    curTargetTicks.AoeAbility = NetworkTick.Invalid;
                    curTargetTicks.SkillShotAbility = NetworkTick.Invalid;
                }

                // 更新AOE技能冷却遮罩
                if (curTargetTicks.AoeAbility == NetworkTick.Invalid ||
                    currentTick.IsNewerThan(curTargetTicks.AoeAbility))
                {
                    abilityCooldownUIController.UpdateAoeMask(0f);
                }
                else
                {
                    var aoeRemainTickCount = curTargetTicks.AoeAbility.TickIndexForValidTick -
                                             currentTick.TickIndexForValidTick;
                    var fillAmount = (float)aoeRemainTickCount / abilityCooldownTicks.AoeAbility;
                    abilityCooldownUIController.UpdateAoeMask(fillAmount);
                }

                // 更新技能射击冷却遮罩
                if (curTargetTicks.SkillShotAbility == NetworkTick.Invalid ||
                    currentTick.IsNewerThan(curTargetTicks.SkillShotAbility))
                {
                    abilityCooldownUIController.UpdateSkillShotMask(0f);
                }
                else
                {
                    var skillShotRemainTickCount = curTargetTicks.SkillShotAbility.TickIndexForValidTick -
                                             currentTick.TickIndexForValidTick;
                    var fillAmount = (float)skillShotRemainTickCount / abilityCooldownTicks.SkillShotAbility;
                    abilityCooldownUIController.UpdateSkillShotMask(fillAmount);
                }
            }
        }
    }
}