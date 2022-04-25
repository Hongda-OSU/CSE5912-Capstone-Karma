using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemies;
        [SerializeField] private GameObject bosses;
        [SerializeField] private LayerMask layerMask;

        [SerializeField] private float enemyLevel;
        [SerializeField] private float levelUpScale = 3f;
        [SerializeField] private bool isLevelledUp = false;

        private List<GameObject> regularList = new List<GameObject>();
        private List<GameObject> bossList = new List<GameObject>();

        private static EnemyManager instance;
        public static EnemyManager Instance { get { return instance; } }

        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(instance);
            }
            instance = this;

            if (enemies != null)
            {
                foreach (Transform enemy in enemies.transform)
                {
                    if (enemy.gameObject.activeSelf)
                        regularList.Add(enemy.gameObject);
                }
            }


            if (bosses != null)
            {
                foreach (Transform boss in bosses.transform)
                {
                    var enemy = boss.GetComponentInChildren<BossEnemy>().gameObject;
                    if (enemy.gameObject.activeSelf)
                        bossList.Add(enemy);
                }
            }

        }
        private void Update()
        {
            if (GameStateController.Instance.karmicLevel > 0 && !isLevelledUp)
            {
                isLevelledUp = true;
                enemyLevel = GameStateController.Instance.karmicLevel;
                LevelupAll(GameStateController.Instance.karmicLevel);
                Debug.Log("Enemies have been leveled up to Lv. " + GameStateController.Instance.karmicLevel);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {

                foreach (GameObject enemy in regularList)
                {
                    enemy.GetComponent<Enemy>().Kill();
                }
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                foreach (GameObject enemy in bossList)
                {
                    enemy.GetComponent<Enemy>().Kill();
                }
            }
        }

        public void ResetEnemiesInScene()
        {
            foreach (GameObject enemy in regularList)
            {
                enemy.SetActive(false);
                enemy.SetActive(true);
                enemy.GetComponent<Enemy>().ResetEnemy();
            }
            foreach (GameObject enemy in bossList)
            {
                enemy.SetActive(false);
                enemy.SetActive(true);
                enemy.GetComponent<Enemy>().ResetEnemy();
            }
        }

        private void LevelupAll(int level)
        {
            foreach (GameObject enemy in regularList)
            {
                enemy.GetComponent<Enemy>().LevelUp(level, levelUpScale);
            }
            foreach (GameObject enemy in bossList)
            {
                enemy.GetComponent<Enemy>().LevelUp(level, levelUpScale);
            }
        }

        public List<GameObject> GetEnemiesInView(float distance)
        {
            List<GameObject> result = new List<GameObject>();

            var planes = GeometryUtility.CalculateFrustumPlanes(WeaponManager.Instance.CarriedWeapon.GunCamera);
            foreach (GameObject enemy in regularList)
            {
                if (enemy != null && enemy.tag == "Enemy" && GeometryUtility.TestPlanesAABB(planes, enemy.GetComponent<Collider>().bounds))
                {
                    var pos = enemy.transform.position + Vector3.up * enemy.GetComponentInChildren<Renderer>().bounds.size.y / 2;
                    var dir = (PlayerManager.Instance.Player.transform.position - pos).normalized;
                    if (Physics.Raycast(pos, dir, out RaycastHit hit, distance, layerMask))
                    {
                        if (hit.transform.gameObject == PlayerManager.Instance.Player)
                            result.Add(enemy);
                    }
                }
            }
            return result;
        }

        public List<GameObject> GetEnemiesAroundPlayer(float radius)
        {
            List<GameObject> result = new List<GameObject>();

            foreach (GameObject enemy in regularList)
            {
                float distance = Vector3.Distance(PlayerManager.Instance.Player.transform.position, enemy.transform.position);
                if (distance <= radius)
                    result.Add(enemy);
            }
            return result;
        }


        public List<GameObject> RugularList { get { return regularList; } }
        public List<GameObject> BossList { get { return bossList; } }
    }
}
