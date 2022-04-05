using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class SwordZone : MonoBehaviour
    {
        [SerializeField] private GameObject swordPrefab;

        [SerializeField] private float radius;
        [SerializeField] private int row;
        [SerializeField] private int numberPerRow;

        private int totalNumber;

        private void Awake()
        {
            totalNumber = row * numberPerRow;
        }

        private void Start()
        {
            GameObject go = new GameObject("SwordZone");

            var origin = transform.position;
            var degree = 360f / numberPerRow;

            for (int j = 0; j < row; j++)
            {
                var y = 2f * j * swordPrefab.GetComponent<Renderer>().bounds.size.y;

                for (int i = 0; i < numberPerRow; i++)
                {
                    var radian = Mathf.Deg2Rad * i * degree;
                    var x = Mathf.Cos(radian) * radius;
                    var z = Mathf.Sin(radian) * radius;

                    var position = new Vector3(x, y, z) + origin;

                    GameObject sword = Instantiate(swordPrefab, go.transform);
                    sword.transform.position = position;
                }
            }
        }
    }
}
