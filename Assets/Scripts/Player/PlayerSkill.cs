using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerSkill : MonoBehaviour
    {

        [SerializeField] private int skillPoints = 0;
        [SerializeField] private GameObject skillTree_element;
        //[SerializeField] private GameObject skillVfxPrefabs;
        //private List<GameObject> skillVfxList;

        private static PlayerSkill instance;
        public static PlayerSkill Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            //skillVfxList = new List<GameObject>();
            //foreach (Transform child in skillVfxPrefabs.transform)
            //{
            //    skillVfxList.Add(child.gameObject);
            //}
        }

        private void Start()
        {
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(0, skillTree_element.transform.Find("Buff").Find("FireMastery").GetComponent<Skill>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(9, skillTree_element.transform.Find("Passive").Find("LightningChain").GetComponent<Skill>());
        }
        private void Update()
        {
            //PerformSkills();
        }


        //private void PerformSkills()
        //{
        //    foreach (SkillTree skillTree in SkillsPanelControl.Instance.SkillTreeList)
        //    {
        //        foreach (SkillSlot skillSlot in skillTree.skillSlotList)
        //        {
        //            Skill skill = skillSlot.skill;

        //            PerformSkill(skill);
        //        }
        //    }

        //}

        //private void PerformSkill(Skill skill)
        //{
        //    if (skill.IsReady)
        //    {
        //        switch (skill.Type)
        //        {
        //            case SkillType.passive:
        //                StartCoroutine(skill.PerformEffect());
        //                break;

        //            case SkillType.main:
        //                break;

        //            case SkillType.buff:
        //                StartCoroutine(skill.PerformEffect());
        //                break;
        //        }
        //    }
        //}

        //public GameObject FindVfx(string name)
        //{
        //    foreach (var vfx in skillVfxList)
        //    {
        //        if (vfx.name == name)
        //            return vfx;
        //    }
        //    return null;
        //}
        public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }
    }
}
