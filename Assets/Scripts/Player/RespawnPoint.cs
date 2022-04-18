using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class RespawnPoint : MonoBehaviour
    {
        [SerializeField] private AudioSource activateAudio;
        [SerializeField] private GameObject activateVfxPrefab;

        [SerializeField] private bool isDefualtRespawnPoint = false;

        [SerializeField] private float timeBetweenActivate = 5f;
        [SerializeField] private bool isReady = true;

        private Collider collider3d;

        private void Awake()
        {
            collider3d = GetComponent<Collider>();
            collider3d.isTrigger = true;
        }

        private void Start()
        {
            if (isDefualtRespawnPoint)
            {
                RespawnManager.Instance.CurrentRespawnPoint = this;
            }
        }
        private void Activate()
        {
            DataManager.Instance.Save();

            StartCoroutine(Cooldown());

            GameObject vfx = Instantiate(activateVfxPrefab, transform);
            Destroy(vfx, 10f);

            activateAudio.Play();

            RespawnManager.Instance.CurrentRespawnPoint = this;

            PlayerStats.Instance.Recover();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            if (isReady)
            {
                TipsControl.Instance.PopUp("X", "Activate");

                if (InputManager.Instance.InputSchemes.PlayerActions.ActivateRespawnPoint.WasPressedThisFrame())
                {
                    Activate();
                }
            }
            else
            {
                TipsControl.Instance.PopOff();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            TipsControl.Instance.PopOff();

        }

        private IEnumerator Cooldown()
        {
            isReady = false;

            yield return new WaitForSeconds(timeBetweenActivate);

            isReady = true;
        }
    }
}
