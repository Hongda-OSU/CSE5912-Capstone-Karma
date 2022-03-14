using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Blink_evilGod : EnemySkill
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform pivot;

        public IEnumerator Perform(Vector3 position)
        {
            GameObject blinks = new GameObject("Blinks");

            var origin = Instantiate(prefab, blinks.transform);
            origin.transform.position = pivot.position;
            Destroy(origin, 5f);

            enemy.transform.position = position;

            yield return new WaitForSeconds(Time.deltaTime);

            var target = Instantiate(prefab, blinks.transform);
            target.transform.position = pivot.position;
            Destroy(target, 5f);

            Destroy(blinks, 5f);
        }

        public override bool IsPerformingAllowed()
        {
            return true;
        }

    }
}
