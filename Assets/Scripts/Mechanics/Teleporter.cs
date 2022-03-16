using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Teleporter : MonoBehaviour
    {
        [SerializeField] private Transform target;

        [SerializeField] private float risingTime = 3f;
        [SerializeField] private float shootingTime = 4.5f;

        [SerializeField] private AudioSource risingSound;
        [SerializeField] private AudioSource shootingSound;

        [SerializeField] private Transform finalPivot;
        [SerializeField] private Transform startPivot;
        [SerializeField] private Transform powerPivot;

        [SerializeField] private Mesh inactiveMesh;
        [SerializeField] private Mesh activeMesh;

        [SerializeField] private GameObject powerPrefab;
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private GameObject portalPrefab;

        private bool isInitialized = false;
        private bool isActivated = false;
        private bool isUsed = false;

        [SerializeField] private MeshFilter meshFilter;

        private Transform teleporter;

        private void Awake()
        {
            meshFilter.mesh = inactiveMesh;
            teleporter = transform.parent;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player" || isActivated || isInitialized)
                return;

            isInitialized = true;

            StartCoroutine(PlayActivateAnimation());

        }
        private void OnTriggerStay(Collider other)
        {
            if (other.tag != "Player" || !isActivated || isUsed)
                return;

            if (InputManager.Instance.InputSchemes.PlayerActions.Interact.triggered)
                StartCoroutine(TeleportPlayer());
        }

        private IEnumerator TeleportPlayer()
        {
            isUsed = true;

            var player = PlayerManager.Instance.Player;

            GameObject portal = Instantiate(portalPrefab);
            portal.transform.position = player.transform.position - Vector3.up * 5f;

            GameStateController.Instance.SetGameState(GameStateController.GameState.Loading);
            player.GetComponent<CharacterController>().enabled = false;
            yield return new WaitForSeconds(3f);
                
            player.transform.position = target.transform.position;
            FPSMouseLook.Instance.ResetLook();

            player.GetComponent<CharacterController>().enabled = true;

            GameStateController.Instance.SetGameState(GameStateController.GameState.InGame);

            Destroy(portal, 1f);
            isUsed = false;
        }

        private IEnumerator PlayActivateAnimation()
        {
            GameObject circle = Instantiate(circlePrefab);
            circle.transform.position = startPivot.position;

            risingSound.Play();

            Vector3 startPosition = teleporter.transform.position;
            Vector3 endPosition = finalPivot.position;

            float timeSince = 0f;
            while (timeSince < risingTime)
            {
                teleporter.transform.position = Vector3.Slerp(startPosition, endPosition, timeSince / risingTime);

                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            risingSound.Stop();

            GameObject power = Instantiate(powerPrefab);
            power.transform.position = powerPivot.position;

            yield return new WaitForSeconds(shootingTime);

            meshFilter.mesh = activeMesh;
            shootingSound.Play();

            Destroy(circle, 5f);
            Destroy(power, 5f);

            isActivated = true;
        }

    }
}
