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

        [SerializeField] private int[] dropRuneIndex;

        public BossArea bossArea;

        public abstract void TriggerBossFight();
        protected abstract void AwakeAnimationComplete();

        protected override void CalculateAggro()
        {
            base.CalculateAggro();
            playerDetected = isBossFightTriggered;
        }

        protected void DropRune()
        {
            for (int i = 0; i < dropRuneIndex.Length; i++)
            {
                DropoffManager.Instance.DropRune(dropRuneIndex[i], transform.position);
            }
        }

        protected override void Die()
        {
            PlayerStats.Instance.GetExperience(experience);

            DropRune();

            isAlive = false;
            agent.isStopped = true;

            collider3d.enabled = false;

            PlayDeathAnimation();
            BgmControl.Instance.MainAudio.Stop();
            StartCoroutine(GetComponentInChildren<BossInformation>().FadeOut());
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
            bossArea.SetBossDefeated(hasBeenDefeated);
            gameObject.SetActive(!hasBeenDefeated);
        }

        public override void ResetEnemy()
        {
            base.ResetEnemy();

            animator.Play("Inactive");
            
            isBossFightTriggered = false;
            bossArea.isPlayerInside = false;
        }


        public override IEnumerator Freeze(float time)
        {
            isFrozen = true;
            agent.isStopped = true;
            agent.speed = 0;
            animator.speed = 0;

            yield return new WaitForSeconds(time / 2f);

            isFrozen = false;
            agent.isStopped = false;
            agent.speed = agentSpeed;
            animator.speed = 1;

            frozen.Stack = 0;
        }
        public override void SlowDown(float percentage)
        {
            if (!isFrozen)
            {
                agent.speed = agentSpeed * (1 - percentage / 2f);
                animator.speed = 1 - percentage / 2f;
            }
        }

        public bool IsBossFightTriggered { get { return playerDetected; } }
    }
}
