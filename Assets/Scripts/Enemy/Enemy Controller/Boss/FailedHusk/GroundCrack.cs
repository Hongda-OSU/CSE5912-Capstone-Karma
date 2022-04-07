using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class GroundCrack : MonoBehaviour
    {
        [SerializeField] private GameObject crackPrefab;
        [SerializeField] private float size;

        public IEnumerator Perform(Enemy enemy)
        {
            GameObject crack = Instantiate(crackPrefab);
            crack.GetComponent<Damager_collision>().Initialize(enemy);

            crack.transform.position = enemy.transform.position;

            var scale = Vector3.one * size;
            foreach (Transform child in crack.transform)
                child.transform.localScale = scale;


            var main = crackPrefab.GetComponent<ParticleSystem>().main;
            float totalDuration = main.duration + main.startLifetime.constant;
            Destroy(crack, totalDuration / 2);

            yield return null;
        }
    }
}
