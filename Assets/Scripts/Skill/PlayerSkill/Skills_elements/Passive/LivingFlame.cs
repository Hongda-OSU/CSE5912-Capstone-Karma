using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LivingFlame : PlayerSkill
    {
        [Header("LivingFlame")]
        [SerializeField] private Incendiary incendiary;

        [SerializeField] private int baseStack = 1;
        [SerializeField] private int stackPerLevel = 1;

        [SerializeField] private float baseTime = 3f;
        [SerializeField] private float timePerLevel = 1.5f;

        public float radius = 30f;
        public float speed = 5f;

        protected override string GetBuiltSpecific()
        {
            var stk = BuildSpecific("Number of Flames per Tracked Enemy", baseStack, stackPerLevel, "", "flames per enemy");
            var duration = BuildSpecific("Duration", baseTime, timePerLevel, "s", "");
            var rad = BuildSpecific("Radius", radius, 0, "m", "");
            return stk + duration + rad;
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();

            if (level == 1)
            {
                incendiary.AllowEnemyTracking(this);
            }
            return result;
        }

        public float Time { get { return baseTime + timePerLevel * (level - 1); } }
        public int Stack { get { return baseStack + stackPerLevel * (level - 1); } }
    }
}
