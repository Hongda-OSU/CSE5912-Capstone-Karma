using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemies;

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

        public List<GameObject> GetEnemiesInView()
        {
            List<GameObject> result = new List<GameObject>();

            var planes = GeometryUtility.CalculateFrustumPlanes(WeaponManager.Instance.CarriedWeapon.GunCamera);
            foreach (GameObject enemy in enemyList)
            {
                if (enemy != null && enemy.tag == "Enemy" && GeometryUtility.TestPlanesAABB(planes, enemy.GetComponent<Collider>().bounds))
                {
                    result.Add(enemy);
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
