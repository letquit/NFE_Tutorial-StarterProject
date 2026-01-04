using Common;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 初始化角色系统，负责为新角色设置物理属性和团队颜色
    /// </summary>
    /// <remarks>
    /// 该系统在SimulationSystemGroup中优先执行，用于初始化新角色的物理质量和渲染颜色
    /// </remarks>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct InitializeCharacterSystem : ISystem
    {
        /// <summary>
        /// 系统更新方法，处理新角色的初始化逻辑
        /// </summary>
        /// <param name="state">系统状态引用，提供对实体管理器和其他系统资源的访问</param>
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (physicsMass, mobaTeam, newCharacterEntity) in SystemAPI.Query<RefRW<PhysicsMass>, MobaTeam>()
                         .WithAny<NewChampTag>().WithEntityAccess())
            {
                // 设置物理质量的逆惯性为零，使角色在物理模拟中保持稳定
                physicsMass.ValueRW.InverseInertia[0] = 0f;
                physicsMass.ValueRW.InverseInertia[1] = 0f;
                physicsMass.ValueRW.InverseInertia[2] = 0f;

                // 根据团队类型确定角色颜色
                var teamColor = mobaTeam.Value switch
                {
                    TeamType.Blue => new float4(0, 0, 1, 1),
                    TeamType.Red => new float4(1, 0, 0, 1),
                    _ => new float4(1)
                };
                
                // 为角色实体设置基础颜色材质属性
                ecb.SetComponent(newCharacterEntity, new URPMaterialPropertyBaseColor { Value = teamColor });
                
                // 移除新角色标签，表示初始化完成
                ecb.RemoveComponent<NewChampTag>(newCharacterEntity);
            }
            
            // 执行命令缓冲区中的所有操作
            ecb.Playback(state.EntityManager);
        }
    }
}