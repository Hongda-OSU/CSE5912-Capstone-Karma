using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Slash : MonoBehaviour
    {
        [SerializeField] private GameObject slashPrefab;


        public IEnumerator Perform(Enemy enemy, float distance)
        {
            GameObject slash = Instantiate(slashPrefab);
            slash.GetComponent<Damager_collision>().Initialize(enemy);
            slash.transform.position = enemy.transform.position + enemy.transform.forward * distance + Vector3.up * enemy.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            slash.transform.LookAt(2 * slash.transform.position - enemy.transform.position);

            var main = slash.GetComponent<ParticleSystem>().main; 
            float totalDuration = main.duration + main.startLifetime.constant;
            Destroy(slash, totalDuration / 2);

            yield return null;
        }
    }
}
