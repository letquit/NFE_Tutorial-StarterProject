using Common;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 薄客户端入口系统，负责在薄客户端模拟环境中创建和配置薄客户端相关的实体和组件
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct ThinClientEntrySystem : ISystem
    {
        /// <summary>
        /// 系统创建时的初始化方法
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
        }

        /// <summary>
        /// 系统更新方法，执行薄客户端的初始化逻辑
        /// </summary>
        /// <param name="state">系统状态引用</param>
        public void OnUpdate(ref SystemState state)
        {
            // 禁用当前系统以防止重复执行
            state.Enabled = false;

            // 创建薄客户端虚拟实体并添加必要的组件
            var thinClientDummy = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<ChampMoveTargetPosition>(thinClientDummy);
            state.EntityManager.AddBuffer<InputBufferData<ChampMoveTargetPosition>>(thinClientDummy);

            // 设置命令目标组件，将连接实体与薄客户端虚拟实体关联
            var connectionEntity = SystemAPI.GetSingletonEntity<NetworkId>();
            SystemAPI.SetComponent(connectionEntity, new CommandTarget { targetEntity = thinClientDummy });
            var connectionId = SystemAPI.GetSingleton<NetworkId>().Value;
            state.EntityManager.AddComponentData(thinClientDummy, new GhostOwner { NetworkId = connectionId });
            
            // 创建客户端队伍请求实体
            var thinClientRequestEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(thinClientRequestEntity, new ClientTeamRequest
            {
                Value = TeamType.AutoAssign
            });

            // 为薄客户端虚拟实体配置输入属性，包括随机数生成器、计时器范围和位置边界
            state.EntityManager.AddComponentData(thinClientDummy, new ThinClientInputProperties
            {
                Random = Random.CreateFromIndex((uint)connectionId),
                Timer = 0f,
                MinTimer = 1f,
                MaxTimer = 10f,
                MinPosition = new float3(-50f, 0f, -50f),
                MaxPosition = new float3(50f, 0f, 50f)
            });
        }
    }
}