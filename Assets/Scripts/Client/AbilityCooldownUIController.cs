using UnityEngine;
using UnityEngine.UI;

namespace TMG.NFE_Tutorial
{
    /// <summary>
    /// 能力冷却UI控制器，负责管理技能冷却遮罩的显示
    /// </summary>
    public class AbilityCooldownUIController : MonoBehaviour
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        public static AbilityCooldownUIController Instance;
        
        /// <summary>
        /// 范围技能冷却遮罩
        /// </summary>
        [SerializeField] private Image _aoeAbilityMask;
        
        /// <summary>
        /// 技能射击冷却遮罩
        /// </summary>
        [SerializeField] private Image _skillShotAbilityMask;

        /// <summary>
        /// 初始化单例实例
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// 初始化冷却遮罩的填充量为0
        /// </summary>
        private void Start()
        {
            _aoeAbilityMask.fillAmount = 0f;
            _skillShotAbilityMask.fillAmount = 0f;
        }

        /// <summary>
        /// 更新范围技能冷却遮罩的填充量
        /// </summary>
        /// <param name="fillAmount">填充量，范围0-1</param>
        public void UpdateAoeMask(float fillAmount)
        {
            _aoeAbilityMask.fillAmount = fillAmount;
        }

        /// <summary>
        /// 更新技能射击冷却遮罩的填充量
        /// </summary>
        /// <param name="fillAmount">填充量，范围0-1</param>
        public void UpdateSkillShotMask(float fillAmount)
        {
            _skillShotAbilityMask.fillAmount = fillAmount;
        }
    }
}