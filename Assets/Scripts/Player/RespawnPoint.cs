using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class RespawnPoint : MonoBehaviour
    {
        [SerializeField] private AudioSource activateAudio;
        [SerializeField] private GameObject activateVfxPrefab;

        [SerializeField] private float timeBetweenActivate = 5f;
        [SerializeField] private bool isReady = true;

        private Collider collider3d;

        private void Awake()
        {
            collider3d = GetComponent<Collider>();
            collider3d.isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            TipsControl.Instance.PopUpTip("X", "Activate");

            if (isReady && InputManager.Instance.InputSchemes.PlayerActions.ActivateRespawnPoint.triggered)
            {
                StartCoroutine(Cooldown());

                GameObject vfx = Instantiate(activateVfxPrefab, transform);
                Destroy(vfx, 10f);

                activateAudio.Play();

                RespawnManager.Instance.CurrentRespawnPoint = this;

                Debug.Log("to-do: recover player");
                // recover player
            }
        }

        private IEnumerator Cooldown()
        {
            isReady = false;

            yield return new WaitForSeconds(timeBetweenActivate);

            isReady = true;
        }
    }
}
