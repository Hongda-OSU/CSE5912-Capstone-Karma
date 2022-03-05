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
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(0, skillTree_element.transform.GetComponentInChildren<FireAscension>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(1, skillTree_element.transform.GetComponentInChildren<CryoAscension>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(2, skillTree_element.transform.GetComponentInChildren<ElectroAscension>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(3, skillTree_element.transform.GetComponentInChildren<VenomAscension>());

            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(4, skillTree_element.transform.GetComponentInChildren<FireMastery>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(5, skillTree_element.transform.GetComponentInChildren<CryoMastery>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(6, skillTree_element.transform.GetComponentInChildren<ElectroMastery>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(7, skillTree_element.transform.GetComponentInChildren<VenomMastery>());

            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(8, skillTree_element.transform.GetComponentInChildren<FireIntellection>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(9, skillTree_element.transform.GetComponentInChildren<CryoIntellection>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(10, skillTree_element.transform.GetComponentInChildren<ElectroIntellection>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(11, skillTree_element.transform.GetComponentInChildren<VenomIntellection>());


            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(4, skillTree_element.GetComponentInChildren<Crystallization>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(5, skillTree_element.GetComponentInChildren<FrostBloodline>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(6, skillTree_element.GetComponentInChildren<FrostBite>());

            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(8, skillTree_element.GetComponentInChildren<LightningBolt>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(9, skillTree_element.GetComponentInChildren<LightningChain>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(10, skillTree_element.GetComponentInChildren<Detonation>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(11, skillTree_element.GetComponentInChildren<Smite>());
        }

        public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }
    }
}
