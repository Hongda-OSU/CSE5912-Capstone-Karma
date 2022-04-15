using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Eternity : PlayerSkill
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

        protected override string GetBuiltSpecific()
        {
            var time = BuildSpecific("Duration", baseTime, timePerLevel, "s", "");
            var rad = BuildSpecific("Radius", baseRadius, radiusPerLevel, "m", "");
            var heal = BuildSpecific("Healing", baseHeal * 100, healPerLevel * 100, "%", "of player's max health");
            var trigger = BuildSpecific("Trigger Health", triggerHealthPercentage * 100, 0, "%", "of player's max health");
            var cd = BuildSpecific("Cooldown", cooldown, 0, "s", "");
            return time + rad + heal + trigger + cd;
        }

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

            float hitRadius = baseRadius + radiusPerLevel * (level - 1);

            GameObject vfx = Instantiate(vfxPrefab);
            var position = PlayerManager.Instance.Player.transform.position;
            vfx.transform.position = new Vector3(position.x, position.y - 1f, position.z); // hard coded
            vfx.transform.localScale = Vector3.one * hitRadius / 0.9f; // hard coded

            float heal = baseHeal + healPerLevel * (level - 1);
            PlayerStats.Instance.Health += heal * PlayerStats.Instance.MaxHealth;

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
                    if (enemy == null || !enemy.IsAlive)
                        continue;

                    enemy.Frozen.StackUp();
                }
            }
            Destroy(vfx);
        }
    }
}
