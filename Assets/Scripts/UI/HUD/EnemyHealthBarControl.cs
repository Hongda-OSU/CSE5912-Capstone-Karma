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
            foreach (var child in EnemyManager.Instance.EnemyList)
            {
                var healthBar = child.GetComponentInChildren<EnemyHealthBar>();

                if (enabled)
                    healthBar.Root.style.display = DisplayStyle.Flex;
                else
                    healthBar.Root.style.display = DisplayStyle.None;
            }
        }
    }
}
