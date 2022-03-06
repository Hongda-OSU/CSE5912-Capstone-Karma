using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class IncendiaryFireDamager : MonoBehaviour
    {
        [SerializeField] float radius = 0.75f;
        [SerializeField] float yOffset = 0.5f;

        public IEnumerator Perform(Incendiary owner, float time)
        {
            float timeSince = 0f;
            while (timeSince < time)
            {
                timeSince += 1f;
                yield return new WaitForSeconds(1f);

                float hitRadius = transform.localScale.x * radius;

                Vector3 position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z); 

                Collider[] hitColliders = Physics.OverlapSphere(position, hitRadius);
                foreach (var hitCollider in hitColliders)
                {
                    hitCollider.TryGetComponent(out Enemy enemy);
                    if (enemy == null || !enemy.IsAlive)
                        continue;

                    owner.PerformDamage(enemy);
                }

            }
            Destroy(gameObject);
        }

    }
}
