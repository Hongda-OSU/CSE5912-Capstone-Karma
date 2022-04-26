using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{ 
    public class RespawnManager : MonoBehaviour
    {
        [SerializeField] private AudioSource deathAudio;

        [SerializeField] private RespawnPoint currentRespawnPoint;

        private GameObject currentSoulPoint;

        [SerializeField] private GameObject deathVfxPrefab;
        [SerializeField] private float timeToRespawn = 8f;

        public GameObject soulPointPrefab;
        private Camera deathCamera;


        private static RespawnManager instance;
        public static RespawnManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            deathCamera = GetComponent<Camera>();
            deathCamera.enabled = false;
        }

        public void RespawnPlayerToLast()
        {
            StartCoroutine(PerformRespawnProcess());
        }

        private IEnumerator PerformRespawnProcess()
        {
            var player = PlayerManager.Instance.Player;
            var currWeapon = WeaponManager.Instance.CarriedWeapon;
            var deathPosition = player.transform.position;

            //EnemyManager.Instance.FreezeAll();

            deathAudio.Play();
            BgmControl.Instance.SmoothStopMusic();
            DeathPanelController.Instance.Display(true);

            currWeapon.Display(false);

            player.layer = LayerMask.NameToLayer("IgnoredByEnemy");

            GameObject deathVfx = Instantiate(deathVfxPrefab);
            deathVfx.transform.position = currWeapon.transform.position;
            Destroy(deathVfx, 10f);

            GameStateController.Instance.SetGameState(GameStateController.GameState.GameOver);

            float timeSince = 0f;
            while (timeSince < timeToRespawn)
            {
                var delta = Time.deltaTime;


                timeSince += delta;
                yield return new WaitForSeconds(delta);
            }

            // respawn at last activated respawn point
            player.layer = LayerMask.NameToLayer("Player");

            FPSControllerCC.Instance.AllowMoving(false);
            player.transform.position = currentRespawnPoint.transform.position;
            PlayerStats.Instance.IsInvincible = true;
            FPSControllerCC.Instance.AllowMoving(true);

            yield return new WaitForSeconds(Time.deltaTime);

            BgmControl.Instance.PlayCurrentBgm();
            GameStateController.Instance.SetGameState(GameStateController.GameState.InGame);

            DeathPanelController.Instance.Display(false);

            currWeapon.Display(true);

            PlayerStats.Instance.Respawn();

            yield return new WaitForSeconds(Time.deltaTime);

            EnemyManager.Instance.ResetEnemiesInScene();

            yield return new WaitForSeconds(Time.deltaTime);

            PlayerStats.Instance.IsInvincible = false;

            CreateSoulPoint(deathPosition);
        }

        private void CreateSoulPoint(Vector3 position)
        {
            GameObject soul = Instantiate(soulPointPrefab);
            soul.transform.position = position;
            PlayerStats.Instance.Experience = 0f;

            if (currentSoulPoint != null)
                Destroy(currentSoulPoint);

            currentSoulPoint = soul;
        }

        public RespawnPoint CurrentRespawnPoint { get { return currentRespawnPoint; } set { currentRespawnPoint = value; } }
    }
}
