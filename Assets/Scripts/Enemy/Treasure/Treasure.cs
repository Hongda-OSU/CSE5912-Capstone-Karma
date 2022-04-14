using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Treasure : RegularEnemy
    {
        [SerializeField] private GameObject deathVfxPrefab;


        protected override void HandlePatrol()
        {
        }

        protected override void HandleWander()
        {
        }

        protected override void PerformActions()
        {
        }

        protected override void PlayDeathAnimation()
        {
            var deathVfx = Instantiate(deathVfxPrefab);
            deathVfx.transform.position = transform.position;

            Destroy(deathVfx, 5f);
        }

        protected override void Die()
        {
            // remove enemy from enemy list and destroy
            isAlive = false;
            agent.isStopped = true;

            collider3d.enabled = false;

            StartCoroutine(WaitAndDisable(gameObject, 0f));

            PlayDeathAnimation();
        }
    }
}
