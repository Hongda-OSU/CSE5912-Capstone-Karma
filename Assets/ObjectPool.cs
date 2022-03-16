using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        [SerializeField] private Queue<GameObject> pool;

        [SerializeField] private int size = 5;

        private void Start()
        {
            for (int i = 0; i < size; i++)
            {
                GameObject go = Instantiate(prefab);
                pool.Enqueue(go);
                go.SetActive(false);
            }
        }

        public GameObject Get()
        {
            if (pool.Count > 0)
            {
                var go = pool.Dequeue();
                go.SetActive(true);
                return go;
            }
            else
            {
                var go = Instantiate(prefab);
                return go;
            }
        }

        public void Return(GameObject go)
        {
            pool.Enqueue(go);
            go.SetActive(false);
        }
    }
}
