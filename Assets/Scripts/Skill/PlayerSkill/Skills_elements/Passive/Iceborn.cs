using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Iceborn : PlayerSkill
    {

        [Header("Iceborn")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float baseRevive = 0.05f;
        [SerializeField] private float revivePerLevel = 0.01f;

        [SerializeField] private AudioSource sfx;

        protected override string GetBuiltSpecific()
        {
            var heal = BuildSpecific("Damage", baseRevive * 100, revivePerLevel * 100, "%", "of player's missing health");
            return heal;
        }

        private void Update()
        {
            if (!isLearned || !isReady)
                return;

            bool isTriggered = false;
            foreach (var target in EnemyManager.Instance.RugularList)
            {
                if (target.GetComponent<Enemy>().IsFrozen)
                {
                    isTriggered = true;
                    break;
                }
            }
            if (!isTriggered)
                return;

            StartCoroutine(Perform());
        }

        private IEnumerator Perform()
        {
            isReady = false;

            sfx.Play();

            GameObject vfx = Instantiate(vfxPrefab, PlayerManager.Instance.Player.transform);
            vfx.transform.position = PlayerManager.Instance.Player.transform.position;
            Destroy(vfx, 1f);

            float deltaHealth = PlayerStats.Instance.MaxHealth - PlayerStats.Instance.Health;
            float revive = deltaHealth * (baseRevive + revivePerLevel * (level - 1));
            PlayerStats.Instance.Health += revive;

            yield return new WaitForSeconds(1f);

            isReady = true;
        }

    }
}
