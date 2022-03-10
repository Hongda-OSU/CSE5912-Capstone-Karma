using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class DontDestroy : MonoBehaviour
    {
        [SerializeField] private GameObject[] objects;

        private void Awake()
        {
            foreach (var obj in objects)
            {
                DontDestroyOnLoad(obj);
            }
        }

    }
}
