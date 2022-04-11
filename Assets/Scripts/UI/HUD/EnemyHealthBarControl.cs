using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class EnemyHealthBarControl : MonoBehaviour
    {
        private static EnemyHealthBarControl instance;
        public static EnemyHealthBarControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this) 
                Destroy(gameObject);
            instance = this;

        }


        public void DisplayEnemyHealthBars(bool enabled)
        {
            if (EnemyManager.Instance == null)
                return;

            foreach (var child in EnemyManager.Instance.EnemyList)
            {
                if (!child.activeSelf)
                    continue;

                UI healthBar = child.GetComponentInChildren<EnemyHealthBar>();
                if (healthBar == null)
                    healthBar = child.GetComponentInChildren<BossInformation>();

                if (enabled)
                    healthBar.Root.style.display = DisplayStyle.Flex;
                else
                    healthBar.Root.style.display = DisplayStyle.None;
            }
        }
    }
}
