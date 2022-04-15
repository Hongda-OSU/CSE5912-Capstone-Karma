using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Pandemic : PlayerSkill
    {

        [Header("Pandemic")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float baseTime = 10f;
        [SerializeField] private float timePerLevel = 2f;

        [SerializeField] private float baseRadius = 3f;
        [SerializeField] private float radiusPerLevel = 0.5f;

        protected override string GetBuiltSpecific()
        {
            var time = BuildSpecific("Duration", baseTime, timePerLevel, "s", "");
            var rad = BuildSpecific("Radius", baseRadius, radiusPerLevel, "m", "");
            var cd = BuildSpecific("Cooldown", cooldown, 0, "s", "");
            return time + rad + cd;
        }
        private void Update()
        {
            if (EnemyManager.Instance == null)
                return;
    
            foreach (var obj in EnemyManager.Instance.EnemyList)
            {
                var enemy = obj.GetComponent<Enemy>();
                if (!isLearned || enemy.Infected.Stack < enemy.Infected.MaxStack)
                    continue;

                StartCoroutine(Perform(enemy));
            }
        }

        private IEnumerator Perform(Enemy target)
        {
            if (!isReady)
                yield break;

            StartCoolingdown();

            float hitRadius = baseRadius + radiusPerLevel * (level - 1);

            GameObject vfx = Instantiate(vfxPrefab);

            var position = target.transform.position;
            vfx.transform.position = position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            vfx.transform.localScale = Vector3.one * hitRadius * 0.01f / 2f; // hard coded

            Vector3 hitPosition = vfx.transform.position;

            float timeSince = 0f;
            float totalTime = baseTime + timePerLevel * (level - 1);
            while (timeSince < totalTime)
            {
                timeSince += 1f;
                yield return new WaitForSeconds(1f);

                Collider[] hitColliders = Physics.OverlapSphere(hitPosition, hitRadius);

                foreach (var hitCollider in hitColliders)
                {
                    hitCollider.TryGetComponent(out Enemy enemy);
                    if (enemy == null || !enemy.IsAlive || enemy == target)
                        continue;

                    enemy.Infected.StackUp();
                }
            }
            Destroy(vfx);
        }
    }
}
