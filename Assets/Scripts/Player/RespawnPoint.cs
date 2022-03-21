using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class RespawnPoint : MonoBehaviour
    {
        [SerializeField] private bool isActivated = false;

        private Collider collider3d;

        private void Awake()
        {
            collider3d = GetComponent<Collider>();
            collider3d.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            isActivated = true;

            RespawnManager.Instance.CurrentRespawnPoint = this;
        }
    }
}
