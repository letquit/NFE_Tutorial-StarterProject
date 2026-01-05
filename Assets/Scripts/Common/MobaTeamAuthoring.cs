using Common;
using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// MOBA游戏团队类型授权组件，用于在Unity编辑器中设置实体的团队归属
    /// </summary>
    public class MobaTeamAuthoring : MonoBehaviour
    {
        /// <summary>
        /// 指定该实体所属的团队类型
        /// </summary>
        public TeamType MobaTeam;
        
        /// <summary>
        /// MOBA团队授权烘焙器，负责将授权组件转换为运行时实体组件
        /// </summary>
        public class MobaTeamBaker : Baker<MobaTeamAuthoring>
        {
            /// <summary>
            /// 将授权组件烘焙为实体组件
            /// </summary>
            /// <param name="authoring">MobaTeamAuthoring类型的授权组件实例</param>
            public override void Bake(MobaTeamAuthoring authoring)
            {
                // 创建动态实体并添加MOBA团队组件
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MobaTeam { Value = authoring.MobaTeam });
            }
        }
    }
}