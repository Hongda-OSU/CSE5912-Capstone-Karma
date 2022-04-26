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

        [SerializeField] private bool isPlayerInsde = false;

        [SerializeField] private BossEnemy[] bossesToActivate;

        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool isUsed = false;
        [SerializeField] private GameObject portalPrefab;

        [SerializeField] private GameObject indicator;

        private static ForwardToNextLevel instance;
        public static ForwardToNextLevel Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            instance = this;

            GetComponent<Collider>().enabled = true;
            GetComponent<Collider>().isTrigger = true;

            indicator.SetActive(isActivated);
        }

        

        private void OnTriggerStay(Collider other)
        {
            if (other.tag != "Player" || !isActivated || isUsed)
                return;

            TipsControl.Instance.PopUp("Z", "Move to Next Level");

            isPlayerInsde = true;
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.tag != "Player" || !isActivated || isUsed)
                return;

            TipsControl.Instance.PopOff();
            isPlayerInsde = false;
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

            DataManager.Instance.Save();

            yield return new WaitForSeconds(3f);

            SceneLoader.Instance.SetPositionOnLoad(nextLevelPosition);
            SceneLoader.Instance.LoadLevel(nextLevelIndex, GameStateController.Instance.karmicLevel);

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
            //// test
            //if (Input.GetKeyDown(KeyCode.P))
            //    StartCoroutine(PlayGameEnding());
            if (!isActivated)
            {
                bool trigger = true;
                foreach (var boss in bossesToActivate)
                    if (boss.IsAlive)
                        trigger = false;
                Activate(trigger);
            }

            if (isPlayerInsde && isActivated && InputManager.Instance.InputSchemes.PlayerActions.Teleport.WasPerformedThisFrame())
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
        private IEnumerator PlayGameEnding()
        {
            if (isUsed)
                yield break;
            isUsed = true;

            DontDestroyOnLoad(this);

            DataManager.Instance.Save();

            BgmControl.Instance.SmoothMusicVolume(0f);


            GameStateController.Instance.SetGameState(GameStateController.GameState.GameOver);
            GameStateController.Instance.karmicLevel++;
            GameStateController.Instance.ResetBosses();

            SceneLoader.Instance.LoadLevel(0, GameStateController.Instance.karmicLevel);

            DontDestroy.Instance.Destroy();

            while (SceneManager.GetActiveScene().buildIndex != 0)
                yield return new WaitForSeconds(Time.deltaTime);

            StartSceneMenu.Instance.PlayGameEnding();

            Destroy(gameObject);
        }

        public void Activate(bool enabled)
        {
            isActivated = enabled;
            indicator.SetActive(enabled);
        }
    }
}
