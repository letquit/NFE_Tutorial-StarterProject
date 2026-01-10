using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 初始化本地玩家角色系统，为本地控制的幽灵实体添加角色标签和移动目标位置组件
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct InitializeLocalChampSystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            // 仅在存在NetworkId组件时运行系统
            state.RequireForUpdate<NetworkId>();
        }

        /// <summary>
        /// 系统更新方法，为本地拥有的幽灵实体添加角色标签和移动目标位置组件
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // 查询本地拥有的、没有OwnerChampTag组件的变换组件，并为它们添加标签和目标位置
            foreach (var (transform, entity) in SystemAPI.Query<LocalTransform>().WithAll<GhostOwnerIsLocal>()
                         .WithNone<OwnerChampTag>().WithEntityAccess())
            {
                ecb.AddComponent<OwnerChampTag>(entity);
                ecb.SetComponent(entity, new ChampMoveTargetPosition { Value = transform.Position });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}