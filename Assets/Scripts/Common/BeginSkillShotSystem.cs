using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 技能射击系统的开始处理系统，负责处理技能射击的冷却、瞄准和发射逻辑
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginSkillShotSystem : ISystem
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
        /// 系统更新方法，处理技能射击的各个阶段逻辑
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var netTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!netTime.IsFirstTimeFullyPredictingTick) return;
            var currentTick = netTime.ServerTick;
            var isServer = state.WorldUnmanaged.IsServer();

            // 检查技能射击冷却状态并标记需要瞄准的技能
            foreach (var skillShot in SystemAPI.Query<SkillShotAspect>().WithAll<Simulate>().WithNone<AimSkillShotTag>())
            {
                var isOnCooldown = true;

                for (var i = 0u; i < netTime.SimulationStepBatchSize; i++)
                {
                    var testTick = currentTick;
                    testTick.Subtract(i);

                    if (!skillShot.CooldownTargetTicks.GetDataAtTick(currentTick, out var curTargetTicks))
                    {
                        curTargetTicks.SkillShotAbility = NetworkTick.Invalid;
                    }

                    if (curTargetTicks.SkillShotAbility == NetworkTick.Invalid ||
                        !curTargetTicks.SkillShotAbility.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }
                
                if (isOnCooldown) continue;
                
                if (!skillShot.BeginAttack) continue;
                ecb.AddComponent<AimSkillShotTag>(skillShot.ChampionEntity);
                
                if (isServer || !SystemAPI.HasComponent<OwnerChampTag>(skillShot.ChampionEntity)) continue;
                var skillShotUIPrefab = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().SkillShot;
                var newSkillShotUI =
                    Object.Instantiate(skillShotUIPrefab, skillShot.AttackPosition, Quaternion.identity);
                ecb.AddComponent(skillShot.ChampionEntity, new SkillShotUIReference { Value = newSkillShotUI });
            }

            // 确认攻击并实例化技能实体，同时处理冷却时间设置
            foreach (var skillShot in SystemAPI.Query<SkillShotAspect>().WithAll<AimSkillShotTag, Simulate>())
            {
                if (!skillShot.ConfirmAttack) continue;
                var skillShotAbility = ecb.Instantiate(skillShot.AbilityPrefab);
                
                var newPosition = skillShot.SpawnPosition;
                ecb.SetComponent(skillShotAbility, newPosition);
                ecb.SetComponent(skillShotAbility, skillShot.Team);
                ecb.RemoveComponent<AimSkillShotTag>(skillShot.ChampionEntity);
                
                if (isServer) continue;
                skillShot.CooldownTargetTicks.GetDataAtTick(currentTick, out var curTargetTicks);

                var newCooldownTargetTick = currentTick;
                newCooldownTargetTick.Add(skillShot.CooldownTicks);
                curTargetTicks.SkillShotAbility = newCooldownTargetTick;

                var nextTick = currentTick;
                nextTick.Add(1u);
                curTargetTicks.Tick = nextTick;

                skillShot.CooldownTargetTicks.AddCommandData(curTargetTicks);
            }

            // 用户施法后的清理UI
            foreach (var (abilityInput, skillShotUIReference, entity) in SystemAPI
                         .Query<AbilityInput, SkillShotUIReference>().WithAll<Simulate>().WithEntityAccess())
            {
                if (!abilityInput.ConfirmSkillShotAbility.IsSet) continue;
                Object.Destroy(skillShotUIReference.Value);
                ecb.RemoveComponent<SkillShotUIReference>(entity);
            }

            // 如果玩家实体被摧毁，清理UI
            foreach (var (skillShotUIReference, entity) in SystemAPI.Query<SkillShotUIReference>().WithAll<Simulate>()
                         .WithNone<LocalTransform>().WithEntityAccess())
            {
                Object.Destroy(skillShotUIReference.Value);
                ecb.RemoveComponent<SkillShotUIReference>(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}