using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class IncendiaryFireDamager : MonoBehaviour
    {
        [SerializeField] float radius = 0.75f;
        [SerializeField] float timeSince = 0f;

        public IEnumerator Perform(Incendiary incendiary, float time, bool spreadToEnemy, Enemy enemyToSpread)
        {
            while (timeSince < time)
            {
                timeSince += 1f;
                yield return new WaitForSeconds(1f);

                float hitRadius = transform.localScale.x * radius;

                Vector3 position = new Vector3(transform.position.x, transform.position.y + transform.localScale.x / 2, transform.position.z); 

                Collider[] hitColliders = Physics.OverlapSphere(position, hitRadius);

                foreach (var hitCollider in hitColliders)
                {
                    hitCollider.TryGetComponent(out Enemy enemy);
                    if (enemy == null || !enemy.IsAlive)
                        continue;

                    incendiary.PerformDamage(enemy);

                    if (spreadToEnemy && !incendiary.burningEnemyList.Contains(enemy))
                    {
                        incendiary.CreateFlame(enemy);

                        incendiary.burningEnemyList.Add(enemy);
                    }
                    else if (incendiary.burningEnemyList.Contains(enemy) && enemy != enemyToSpread)
                    {
                        enemy.GetComponentInChildren<IncendiaryFireDamager>().Refresh();
                    }
                }
            }
            Refresh();

            if (enemyToSpread != null)
                incendiary.burningEnemyList.Remove(enemyToSpread);

            Destroy(gameObject);
        }
        
        private void Refresh()
        {
            timeSince = 0f;
        }
    }
}
