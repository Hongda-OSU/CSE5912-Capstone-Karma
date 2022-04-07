using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class GroundCrack : EnemySkill
    {
        [SerializeField] private GameObject crackPrefab;
        [SerializeField] private float size;

        public override IEnumerator Perform()
        {
            GameObject crack = Instantiate(crackPrefab);
            crack.GetComponent<Damager_collision>().Initialize(enemy);

            crack.transform.position = enemy.transform.position;

            var scale = Vector3.one * size;
            crack.transform.localScale = scale;
            foreach (Transform child in crack.transform)
                child.transform.localScale = scale;


            var main = crackPrefab.GetComponent<ParticleSystem>().main;
            float totalDuration = main.duration + main.startLifetime.constant;
            Destroy(crack, totalDuration / 2);

            yield return null;
        }

        public override bool IsPerformingAllowed()
        {
            return true;
        }
    }
}
