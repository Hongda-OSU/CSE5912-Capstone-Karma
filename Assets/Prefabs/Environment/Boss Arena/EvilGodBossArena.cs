using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class EvilGodBossArena : MonoBehaviour
    {
        [Header("Movable Objects")]
        [SerializeField] private GameObject sky;
        [SerializeField] private GameObject doors_1;
        [SerializeField] private GameObject doors_2;
        [SerializeField] private GameObject doors_3;

        void Start()
        {
        
        }

        void Update()
        {
            sky.transform.Rotate(new Vector3(0f, 1000f * Time.deltaTime, 0f), Space.Self);
            doors_1.transform.Rotate(new Vector3(0f, -0.3f, 0f), Space.Self);
            doors_2.transform.Rotate(new Vector3(0f, 0.3f, 0f), Space.Self);
            doors_3.transform.Rotate(new Vector3(0f, 1.5f, 0f), Space.Self);
        }
    }
}
