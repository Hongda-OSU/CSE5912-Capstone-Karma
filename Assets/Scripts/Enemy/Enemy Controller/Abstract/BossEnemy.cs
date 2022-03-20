using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class BossEnemy : EliteEnemy
    {
        [Header("Boss Enemy")]
        [SerializeField] protected bool isBossFightTriggered = false;

        [SerializeField] private GameObject bossDeathVfxPrefab;

        public abstract void TriggerBossFight();

        protected override void CalculateAggro()
        {
            base.CalculateAggro();
            playerDetected = isBossFightTriggered;
        }

        protected override void Die()
        {
            PlayerStats.Instance.GetExperience(experience);

            // remove enemy from enemy list and destroy
            isAlive = false;
            agent.isStopped = true;

            collider3d.enabled = false;

            PlayDeathAnimation();
            IngameAudioControl.Instance.MainAudio.Stop();
            GetComponentInChildren<BossInformation>().Fadeout();
        }

        protected void DeathAnimationComplete()
        {
            StartCoroutine(RemoveAndDestroy(gameObject, 0f));

            GameObject vfx = Instantiate(bossDeathVfxPrefab);
            vfx.transform.position = transform.position + Vector3.up * GetComponentInChildren<Renderer>().bounds.size.y / 2;
            Destroy(vfx, 10f);

            IngameAudioControl.Instance.PlayBossDefeated();
        }

        public bool IsBossFightTriggered { get { return playerDetected; } }
    }
}
