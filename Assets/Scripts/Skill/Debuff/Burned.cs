using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Burned : Debuff
    {
        [SerializeField] private float deltaTime = 1f;


        protected override IEnumerator Perform()
        {
            while (timeSince < duration)
            {
                timeSince += deltaTime;
                yield return new WaitForSeconds(deltaTime);

                float final = PlayerStats.Instance.BurnedDamagePerStack * stack;

                Damage damage = new Damage(final, Element.Type.Fire, PlayerStats.Instance, target);
                PlayerManager.Instance.PerformSkillDamage(target, damage);
            }
            stack = 0;
        }


    }
}
