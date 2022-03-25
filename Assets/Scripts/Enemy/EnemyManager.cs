using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemies;
        [SerializeField] private LayerMask layerMask;

        private List<GameObject> enemyList;


        private static EnemyManager instance;
        public static EnemyManager Instance { get { return instance; } }

        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(instance);
            }
            instance = this;

            enemyList = new List<GameObject>();
            foreach (Transform enemy in enemies.transform)
            {
                if (enemy.gameObject.activeSelf)
                    enemyList.Add(enemy.gameObject);
            }
        }

        public void ResetEnemiesInScene()
        {
            foreach (GameObject enemy in enemyList)
            {
                enemy.SetActive(false);
                enemy.SetActive(true);
                enemy.GetComponent<Enemy>().ResetEnemy();
            }
        }

        public List<GameObject> GetEnemiesInView(float distance)
        {
            List<GameObject> result = new List<GameObject>();

            var planes = GeometryUtility.CalculateFrustumPlanes(WeaponManager.Instance.CarriedWeapon.GunCamera);
            foreach (GameObject enemy in enemyList)
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

            foreach (GameObject enemy in enemyList)
            {
                float distance = Vector3.Distance(PlayerManager.Instance.Player.transform.position, enemy.transform.position);
                if (distance <= radius)
                    result.Add(enemy);
            }
            return result;
        }


        public List<GameObject> EnemyList { get { return enemyList; } }
    }
}
