using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    [RequireComponent(typeof(Collider))]
    public class OutOfBoundDamage : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                PlayerStats.Instance.Die();
        }
    }
}
