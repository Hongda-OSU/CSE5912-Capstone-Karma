using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerSkill : MonoBehaviour
    {

        public int skillPoints = 0;

        public List<Skill> skillList_electro;

        private void Awake()
        {
            skillList_electro = new List<Skill>();

            // todo - add skills here

            // test
            // empty skills
            for (int i = 0; i < 5; i++)
            {
                skillList_electro.Add(new Skill());

                if (i != 0)
                    skillList_electro[i].RequiredSkill = skillList_electro[i - 1];
            }
        }

    }
}
