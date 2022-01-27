using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class Weapon : MonoBehaviour
    {
        // in-game properties
        [SerializeField] private float damage;

        // ui related
        public Sprite iconImage;
    }
}
