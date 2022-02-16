using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class EnemyInformationControl : UI
    {
        private static EnemyInformationControl instance;
        public static EnemyInformationControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            Initialize();

        }

        private void Start()
        {
        }

        private void Update()
        {
            var enemyList = EnemyManager.Instance.GetEnemiesInView();
            foreach (var enemy in enemyList)
            {
                Debug.Log(enemy.name);
            }
        }
    }
}
