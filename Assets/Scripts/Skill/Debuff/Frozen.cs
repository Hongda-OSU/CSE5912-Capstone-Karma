using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Frozen : Debuff
    {
         private float deltaTime = 0.1f;

        protected override IEnumerator Perform()
        {
            while (timeSince < duration)
            {
                timeSince += deltaTime;
                yield return new WaitForSeconds(deltaTime);

                float final = PlayerStats.Instance.FrozenSlowdownPerStack * stack;
                target.SlowDown(final);
            }
            stack = 0;
        }

        public override void StackUp()
        {
            if (target.IsFrozen)
                return;

            base.StackUp();
        }
    }
}
