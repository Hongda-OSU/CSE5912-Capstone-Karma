using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Slash : EnemySkill
    {
        [SerializeField] private GameObject slashPrefab;
        [SerializeField] private GameObject prickPrefab;
        [SerializeField] private float distance;
        public int direction = 0;

        public override IEnumerator Perform()
        {
            GameObject slash;
            if (direction != 4)
            {
                slash = Instantiate(slashPrefab);
                slash.transform.position = enemy.transform.position + enemy.transform.forward * distance + Vector3.up * enemy.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            }
            else
            {
                slash = Instantiate(prickPrefab);
                slash.transform.position = enemy.transform.position + enemy.transform.forward * distance / 2 + Vector3.up * enemy.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            }
            slash.GetComponent<Damager_collision>().Initialize(enemy);


            var lookPos = slash.transform.position - enemy.transform.position;
            lookPos.y = 0;
            slash.transform.rotation = Quaternion.LookRotation(lookPos);
            if (direction >= 0 && direction < 4)
            {
                Vector3 eulerRotation = transform.rotation.eulerAngles;
                slash.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, eulerRotation.z + direction * 90 + 180);
            }

            var main = slash.GetComponent<ParticleSystem>().main; 
            float totalDuration = main.duration + main.startLifetime.constant;
            Destroy(slash, totalDuration / 2);

            yield return null;
        }
        public override bool IsPerformingAllowed()
        {
            return true;
        }
    }
}
