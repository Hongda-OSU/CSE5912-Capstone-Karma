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
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(0, skillTree_element.transform.Find("Buff").Find("FireMastery").GetComponent<Skill>());

            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(8, skillTree_element.transform.Find("Passive").Find("LightningBolt").GetComponent<Skill>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(9, skillTree_element.transform.Find("Passive").Find("LightningChain").GetComponent<Skill>());
        }

        public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }
    }
}
