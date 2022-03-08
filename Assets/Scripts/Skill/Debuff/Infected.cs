using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Infected : Debuff
    {
        public float timeLasted = 0f;
        private float deltaTime = 1f;

        protected override IEnumerator Perform()
        {
            while (timeSince < duration)
            {
                if (stack == maxStack)
                    timeLasted += deltaTime;

                timeSince += deltaTime;
                yield return new WaitForSeconds(deltaTime);

                float final = PlayerStats.Instance.InfectedCurrentHealthDamagePerStack * stack;

                Damage damage = new Damage(target.Health * final, Element.Type.Venom, PlayerStats.Instance, target);
                PlayerManager.Instance.PerformSkillDamage(target, damage);
            }
            stack = 0;
            timeLasted = 0f;
        }
    }
}
