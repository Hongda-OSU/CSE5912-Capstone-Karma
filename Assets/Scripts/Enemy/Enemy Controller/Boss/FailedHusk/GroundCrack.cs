using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class GroundCrack : MonoBehaviour
    {
        [SerializeField] private GameObject crackPrefab;

        public IEnumerator Perform(Enemy enemy, float size)
        {
            GameObject crack = Instantiate(crackPrefab);
            crack.GetComponent<Damager_collision>().Initialize(enemy);

            crack.transform.position = enemy.transform.position;

            var scale = Vector3.one * size;
            foreach (Transform child in crack.transform)
                child.transform.localScale = scale;

            Destroy(crack, 10f);

            yield return null;
        }
    }
}
