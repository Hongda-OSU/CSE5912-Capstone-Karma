using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{ 
    public class RespawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject respawnPoints;
        private List<RespawnPoint> respawnPointList;
        [SerializeField] private RespawnPoint currentRespawnPoint;

        [SerializeField] private GameObject deathVfxPrefab;
        [SerializeField] private float timeToRespawn = 8f;

        [SerializeField] private GameObject soulPointPrefab;
        private Camera deathCamera;


        private static RespawnManager instance;
        public static RespawnManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
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

            DeathPanelController.Instance.Display(true);

            currWeapon.Display(false);

            player.layer = LayerMask.NameToLayer("IgnoredByEnemy");

            GameObject deathVfx = Instantiate(deathVfxPrefab);
            deathVfx.transform.position = currWeapon.transform.position;

            GameStateController.Instance.SetGameState(GameStateController.GameState.GameOver);

            float timeSince = 0f;
            while (timeSince < timeToRespawn)
            {
                var delta = Time.deltaTime;


                timeSince += delta;
                yield return new WaitForSeconds(delta);
            }

            player.layer = LayerMask.NameToLayer("Player");

            player.transform.position = currentRespawnPoint.transform.position;

            GameStateController.Instance.SetGameState(GameStateController.GameState.InGame);

            DeathPanelController.Instance.Display(false);

            currWeapon.Display(true);

            PlayerStats.Instance.Respawn();
            EnemyManager.Instance.ResetEnemiesInScene();

            yield return new WaitForSeconds(Time.deltaTime);

            CreateSoulPoint(deathPosition);
        }

        private void CreateSoulPoint(Vector3 position)
        {
            GameObject soul = Instantiate(soulPointPrefab);
            soul.transform.position = position;
            soul.GetComponent<SoulPoint>().Initialize(PlayerStats.Instance.Experience);
            PlayerStats.Instance.Experience = 0f;
        }

        public RespawnPoint CurrentRespawnPoint { get { return currentRespawnPoint; } set { currentRespawnPoint = value; } }
    }
}
