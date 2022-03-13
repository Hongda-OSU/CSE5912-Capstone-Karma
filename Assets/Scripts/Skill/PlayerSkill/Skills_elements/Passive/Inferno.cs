using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Inferno : PlayerSkill
    {
        [Header("Inferno")]
        [SerializeField] Incendiary incendiary;
        public override bool LevelUp()
        {
            bool result = base.LevelUp();

            if (level == 1)
            {
                incendiary.AllowSpreadToEnemy();
            }
            return result;
        }
    }
}
