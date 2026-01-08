using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// NPC攻击组件的Authoring脚本，用于在编辑器中配置NPC的攻击相关参数
    /// </summary>
    public class NpcAttackAuthoring : MonoBehaviour
    {
        /// <summary>
        /// NPC寻找目标的半径范围
        /// </summary>
        public float NpcTargetRadius;
        
        /// <summary>
        /// 攻击发射点的偏移量
        /// </summary>
        public Vector3 FirePointOffset;
        
        /// <summary>
        /// 攻击冷却时间（秒）
        /// </summary>
        public float AttackCooldownTime;
        
        /// <summary>
        /// 攻击预制体对象
        /// </summary>
        public GameObject AttackPrefab;
        
        /// <summary>
        /// 网络代码配置
        /// </summary>
        public NetCodeConfig NetCodeConfig;
        
        /// <summary>
        /// 获取模拟器的Tick速率
        /// </summary>
        public int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;

        /// <summary>
        /// NPC攻击组件的烘焙器，将Authoring组件转换为ECS实体组件
        /// </summary>
        public class NpcAttackBaker : Baker<NpcAttackAuthoring>
        {
            /// <summary>
            /// 将NpcAttackAuthoring组件烘焙为ECS实体组件
            /// </summary>
            /// <param name="authoring">NpcAttackAuthoring组件实例</param>
            public override void Bake(NpcAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new NpcTargetRadius { Value = authoring.NpcTargetRadius });
                AddComponent(entity, new NpcAttackProperties
                {
                    FirePointOffset = authoring.FirePointOffset,
                    CooldownTickCount = (uint)(authoring.AttackCooldownTime * authoring.SimulationTickRate),
                    AttackPrefab = GetEntity(authoring.AttackPrefab, TransformUsageFlags.Dynamic)
                });
                // 添加NPC目标实体组件
                AddComponent<NpcTargetEntity>(entity);
                // 添加NPC攻击冷却缓冲区
                AddBuffer<NpcAttackCooldown>(entity);
            }
        }
    }
}