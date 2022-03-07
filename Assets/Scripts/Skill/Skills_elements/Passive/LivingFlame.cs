using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LivingFlame : Skill
    {
        [Header("LivingFlame")]
        [SerializeField] private Incendiary incendiary;

        public float radius = 30f;
        public float speed = 5f;

        public override bool LevelUp()
        {
            bool result = base.LevelUp();

            if (level == 1)
            {
                incendiary.AllowEnemyTracking(this);
            }
            return result;
        }
    }
}
