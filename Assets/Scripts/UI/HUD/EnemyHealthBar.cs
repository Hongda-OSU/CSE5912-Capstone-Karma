using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class EnemyHealthBar : UI
    {
        [SerializeField] private Camera currentCamera;

        [SerializeField] int i;

        private void Awake()
        {
            i = 0;
        }

        private void Start()
        {
            i = 1;
        }
    }
}
