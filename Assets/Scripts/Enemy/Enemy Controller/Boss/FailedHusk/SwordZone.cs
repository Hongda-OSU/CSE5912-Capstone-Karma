using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class SwordZone : EnemySkill
    {
        [SerializeField] private GameObject swordPrefab;

        [SerializeField] private float delay;
        [SerializeField] private float speed;
        [SerializeField] private float radius;
        [SerializeField] private float height;
        [SerializeField] private float offset;
        [SerializeField] private int row;
        [SerializeField] private int numberPerRow;

        [SerializeField] private LayerMask layerMask;

        public override IEnumerator Perform()
        {
            GameObject go = new GameObject("SwordZone");

            var origin = transform.position;
            var degree = 360f / numberPerRow;

            for (int j = 0; j < row; j++)
            {
                var r = radius + row * j * offset;

                for (int i = 0; i < numberPerRow; i++)
                {
                    var radian = Mathf.Deg2Rad * i * degree;
                    var x = Mathf.Cos(radian) * r;
                    var z = Mathf.Sin(radian) * r;

                    var position = new Vector3(x, height, z) + origin;

                    GameObject sword = Instantiate(swordPrefab, go.transform);
                    sword.GetComponent<Damager_collision>().Initialize(enemy);
                    sword.transform.position = position;
                    
                    StartCoroutine(Drop(sword));

                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        private IEnumerator Drop(GameObject sword)
        {
            var position = sword.transform.position;
            if (Physics.Raycast(sword.transform.position, Vector3.down, out RaycastHit hit, layerMask))
            {
                position = hit.point + Vector3.up * sword.GetComponent<Renderer>().bounds.size.y / 2;
            }

            yield return new WaitForSeconds(delay);

            while (Vector3.Distance(position, sword.transform.position) > 0.01f)
            {
                sword.transform.position = Vector3.MoveTowards(sword.transform.position, position, speed * Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            sword.GetComponent<Damager_collision>().enabled = false;
        }
        public override bool IsPerformingAllowed()
        {
            return true;
        }
    }
}
