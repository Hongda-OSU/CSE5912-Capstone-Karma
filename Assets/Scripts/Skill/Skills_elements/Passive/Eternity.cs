using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Eternity : Skill
    {
        [Header("Eternity")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float baseTime = 5f;
        [SerializeField] private float timePerLevel = 1f;

        [SerializeField] private float baseRadius = 5f;
        [SerializeField] private float radiusPerLevel = 1f;

        [SerializeField] private float baseHeal = 0.3f;
        [SerializeField] private float healPerLevel = 0.1f;

        [SerializeField] private float triggerHealthPercentage = 0.3f;

        private void Update()
        {
            float healthPercentage = PlayerStats.Instance.Health / PlayerStats.Instance.MaxHealth;
            if (!isLearned || healthPercentage > triggerHealthPercentage)
                return;

            StartCoroutine(Perform());
        }

        private IEnumerator Perform()
        {
            if (!isReady)
                yield break;

            StartCoolingdown();

            float radius = baseRadius + radiusPerLevel * (level - 1);

            GameObject vfx = Instantiate(vfxPrefab);
            var position = PlayerManager.Instance.Player.transform.position;
            vfx.transform.position = new Vector3(position.x, position.y - 1f, position.z); // hard coded
            vfx.transform.localScale = Vector3.one * radius / 0.9f; // hard coded

            float heal = baseHeal + healPerLevel * (level - 1);
            PlayerStats.Instance.Health += heal * PlayerStats.Instance.MaxHealth;

            float timeSince = 0f;
            float totalTime = baseTime + timePerLevel * (level - 1);
            while (timeSince < totalTime)
            {
                timeSince += 1f;
                yield return new WaitForSeconds(1f);

                foreach (var enemyObj in EnemyManager.Instance.EnemyList)
                {
                    var enemy = enemyObj.GetComponent<Enemy>();
                    if (!enemy.IsAlive)
                        continue;

                    float distance = Vector3.Distance(vfx.gameObject.transform.position, enemyObj.transform.position);

                    if (distance < radius)
                    {
                        enemy.Frozen.StackUp();
                    }
                }
            }
            Destroy(vfx);
        }
    }
}
