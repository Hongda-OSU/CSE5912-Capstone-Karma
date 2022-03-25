using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class SoulPoint : MonoBehaviour
    {
        [SerializeField] private float experience;
        [SerializeField] private GameObject pickUpVfxPrefab;

        public void Initialize(float experience)
        {
            this.experience = experience;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            PlayerStats.Instance.Experience += experience;
            experience = 0f;
            Destroy(gameObject);

            GameObject vfx = Instantiate(pickUpVfxPrefab);
            vfx.transform.position = transform.position;

            Destroy(vfx, 10f);
        }
    }
}
