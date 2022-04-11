using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    [RequireComponent(typeof(Collider))]
    public class ForwardToNextLevel : MonoBehaviour
    {
        [SerializeField] private int nextLevelIndex;
        [SerializeField] private Vector3 nextLevelPosition;

        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool isUsed = false;
        [SerializeField] private GameObject portalPrefab;

        Collider collider3d;

        private void Awake()
        {
            collider3d = GetComponent<Collider>();
            collider3d.enabled = true;
            collider3d.isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag != "Player" || !isActivated || isUsed)
                return;

            TipsControl.Instance.PopUp("Z", "Move to Next Level");

            if (InputManager.Instance.InputSchemes.PlayerActions.Teleport.triggered)
            {
                StartCoroutine(MoveToNext());
                TipsControl.Instance.PopOff();
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.tag != "Player" || !isActivated || isUsed)
                return;

            TipsControl.Instance.PopOff();
        }

        private IEnumerator MoveToNext()
        {
            isUsed = true;
            PlayerStats.Instance.IsInvincible = true;

            var player = PlayerManager.Instance.Player;

            GameObject portal = Instantiate(portalPrefab);
            portal.transform.position = player.transform.position - Vector3.up * 5f;

            FPSControllerCC.Instance.AllowMoving(false);

            GameStateController.Instance.SetGameState(GameStateController.GameState.Loading);


            yield return new WaitForSeconds(3f);

            SceneLoader.Instance.SetPositionOnLoad(nextLevelPosition);
            SceneLoader.Instance.LoadLevel(nextLevelIndex);

            //player.transform.position = target.transform.position;
            //player.transform.position = to;
            //FPSMouseLook.Instance.ResetLook();

            //FPSControllerCC.Instance.AllowMoving(true);

            //GameStateController.Instance.SetGameState(GameStateController.GameState.InGame);

            //Destroy(portal, 1f);
            //isUsed = false;
        }

        public void Activate()
        {
            isActivated = true;
        }
    }
}
