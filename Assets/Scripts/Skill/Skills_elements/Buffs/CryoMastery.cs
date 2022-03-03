using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class CryoMastery : Skill
    {
        [SerializeField] private float slowPerLevel = 0.04f;

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
