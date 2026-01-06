using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 能力输入系统，负责处理游戏中的能力输入并更新实体的能力输入组件
    /// </summary>
    public partial class AbilityInputSystem : SystemBase
    {
        private MobaInputActions _inputActions;

        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        protected override void OnCreate()
        {
            _inputActions = new MobaInputActions();
        }

        /// <summary>
        /// 系统开始运行时启用输入动作
        /// </summary>
        protected override void OnStartRunning()
        {
            _inputActions.Enable();
        }

        /// <summary>
        /// 系统停止运行时禁用输入动作
        /// </summary>
        protected override void OnStopRunning()
        {
            _inputActions.Disable();
        }
        
        /// <summary>
        /// 系统更新方法，处理能力输入并更新所有实体的能力输入组件
        /// </summary>
        protected override void OnUpdate()
        {
            var newAbilityInput = new AbilityInput();

            // 检测AOE能力按键是否在当前帧被按下
            if (_inputActions.GameplayMap.AoeAblility.WasPressedThisFrame())
            {
                newAbilityInput.AoeAbility.Set();
            }

            if (_inputActions.GameplayMap.SkillShotAbility.WasPressedThisFrame())
            {
                newAbilityInput.SkillShotAbility.Set();
            }

            if (_inputActions.GameplayMap.ConfirmSkillShotAbility.WasPressedThisFrame())
            {
                newAbilityInput.ConfirmSkillShotAbility.Set();
            }

            // 遍历所有具有AbilityInput组件的实体，并更新其能力输入数据
            foreach (var abilityInput in SystemAPI.Query<RefRW<AbilityInput>>())
            {
                abilityInput.ValueRW = newAbilityInput;
            }
        }
    }
}