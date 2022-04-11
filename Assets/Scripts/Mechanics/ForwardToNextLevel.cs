using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    [RequireComponent(typeof(Collider))]
    public class ForwardToNextLevel : MonoBehaviour
    {
        [Tooltip(" -1 if game is over")]
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
                // -1: game is over
                if (nextLevelIndex == -1)
                {
                    StartCoroutine(PlayGameEnding());
                }
                else
                {
                    StartCoroutine(MoveToNext());
                }

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

            DataManager.Instance.Save();

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                StartCoroutine(PlayGameEnding());
        }
        private IEnumerator PlayGameEnding()
        {
            if (isUsed)
                yield break;
            isUsed = true;

            DontDestroyOnLoad(this);

            DataManager.Instance.Save();

            BgmControl.Instance.SmoothMusicVolume(0f);

            SceneLoader.Instance.LoadLevel(0);

            GameStateController.Instance.SetGameState(GameStateController.GameState.GameOver);

            DontDestroy.Instance.Destroy();

            while (SceneManager.GetActiveScene().buildIndex != 0)
                yield return new WaitForSeconds(Time.deltaTime);

            StartSceneMenu.Instance.PlayGameEnding();

            Destroy(this);
        }

        public void Activate()
        {
            isActivated = true;
        }
    }
}
