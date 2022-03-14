using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Shield_evilGod : EnemySkill
    {
        [SerializeField] private GameObject shieldPrefab;
        [SerializeField] private float triggerHealthPercentage = 0.5f;

        public override IEnumerator Perform()
        {
            isReady = false;

            GameObject energyShield = Instantiate(shieldPrefab, transform);
            Shield shield = energyShield.GetComponent<Shield>();

            while (true)
            {
                yield return new WaitForSeconds(Time.deltaTime);

                if (shield.TotalHealth <= 0)
                {
                    Destroy(energyShield);
                    break;
                }
            }
        }

        public override bool IsPerformingAllowed()
        {
            return isReady && enemy.Health < enemy.MaxHealth * triggerHealthPercentage;
        }

    }
}
