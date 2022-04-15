using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class CryoMastery : PlayerSkill
    {
        [SerializeField] private float slowPerLevel = 0.008f;

        protected override string GetBuiltSpecific()
        {
            var value = BuildSpecific("Slow", slowPerLevel * 100, slowPerLevel * 100, "%", "per stack");
            var curr = "\n\nBase slowdown per stack: \n" + PlayerStats.Instance.FrozenSlowdownPerStack * 100 + "%";
            return value + curr;
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                PlayerStats.Instance.FrozenSlowdownPerStack += slowPerLevel;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.FrozenSlowdownPerStack -= slowPerLevel * level;
        }

        public float SlowPerLevel { get { return slowPerLevel; } }
    }
}
