using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CSE5912.PolyGamers.Skill;

namespace CSE5912.PolyGamers
{
    public class PlayerSkill : MonoBehaviour
    {

        [SerializeField] private int skillPoints = 0;


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

        private void Update()
        {
            PlayerStats.Instance.ResetExtras();

            PerformSkills();
        }


        private void PerformSkills()
        {
            foreach (SkillTree skillTree in SkillsPanelControl.Instance.SkillTreeList)
            {
                foreach (SkillSlot skillSlot in skillTree.skillSlotList)
                {
                    Skill skill = skillSlot.skill;

                    PerformSkill(skill);
                }
            }

            //test
            Debug.Log(PlayerStats.Instance.ExtraDamageFactor_fire);
        }

        private void PerformSkill(Skill skill)
        {
            if (skill.IsReady)
            {
                switch (skill.Type)
                {
                    case SkillType.passive:
                        break;

                    case SkillType.main:
                        break;

                    case SkillType.buff:
                        StartCoroutine(skill.PerformEffect());
                        break;
                }
            }
        }

        public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }
    }
}
