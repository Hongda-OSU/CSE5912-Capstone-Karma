using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ExplosiveSkeleton : RegularEnemy
    {
        [Header("Explosion")]
        [SerializeField] private GameObject effect;
        [SerializeField] private GameObject barrel;

        protected override void PerformActions()
        {

        }

        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }

        protected override void HandlePatrol()
        {
            // Do Nothing
        }

        protected override void HandleWander()
        {
            // Do Nothing
        }

        private void Explode() {
            GameObject vfx = Instantiate(effect);
            vfx.transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);

            health = 0;
            Destroy(barrel);
            Destroy(vfx, 10f);
        }
    }
}
