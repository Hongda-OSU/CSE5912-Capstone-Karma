using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Frozen : Debuff
    {
        [SerializeField] private float slowPerStack = 0.05f;
         private float deltaTime = 0.1f;

        protected override IEnumerator Perform()
        {
            while (timeSince < duration)
            {
                timeSince += deltaTime;
                yield return new WaitForSeconds(deltaTime);

                target.SlowDown(slowPerStack * stack);
            }
            stack = 0;
        }
    }
}
