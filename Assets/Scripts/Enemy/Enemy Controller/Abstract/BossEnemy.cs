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

        public BossArea bossArea;

        public abstract void TriggerBossFight();
        protected abstract void AwakeAnimationComplete();

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
            BgmControl.Instance.MainAudio.Stop();
            GetComponentInChildren<BossInformation>().Fadeout();
        }

        protected void DeathAnimationComplete()
        {
            StartCoroutine(WaitAndDisable(gameObject, 0f));

            GameObject vfx = Instantiate(bossDeathVfxPrefab);
            vfx.transform.position = transform.position + Vector3.up * GetComponentInChildren<Renderer>().bounds.size.y / 2;
            Destroy(vfx, 10f);

            BgmControl.Instance.PlayBossDefeated();
        }

        public void SetBossDefeated(bool hasBeenDefeated)
        {
            isBossFightTriggered = hasBeenDefeated;
            isAlive = !hasBeenDefeated;
            bossArea.isBossDefeated = hasBeenDefeated;
            gameObject.SetActive(!hasBeenDefeated);
        }

        public bool IsBossFightTriggered { get { return playerDetected; } }
    }
}
