using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Infected : Debuff
    {

        [SerializeField] private float currentHealthDamagePerStack = 0.01f;
        private float deltaTime = 1f;

        protected override IEnumerator Perform()
        {
            while (timeSince < duration)
            {
                timeSince += deltaTime;
                yield return new WaitForSeconds(deltaTime);

                Damage damage = new Damage(target.Health * currentHealthDamagePerStack * stack, Element.Type.Venom, PlayerStats.Instance, target);
                PlayerManager.Instance.PerformSkillDamage(target, damage);
            }
            stack = 0;
        }
    }
}
