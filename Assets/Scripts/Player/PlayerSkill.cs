using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerSkill : MonoBehaviour
    {

        [SerializeField] private int skillPoints = 0;
        [SerializeField] private GameObject skillTree_element;

        private static PlayerSkill instance;
        public static PlayerSkill Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

        }

        private void Start()
        {
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(0, skillTree_element.transform.GetComponentInChildren<FireMastery>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(1, skillTree_element.transform.GetComponentInChildren<CryoMastery>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(2, skillTree_element.transform.GetComponentInChildren<ElectroMastery>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(3, skillTree_element.transform.GetComponentInChildren<VenomMastery>());

            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(8, skillTree_element.GetComponentInChildren<LightningBolt>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(9, skillTree_element.GetComponentInChildren<LightningChain>());
        }

        public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }
    }
}
