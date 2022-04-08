using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class BossArea : MonoBehaviour
    {
        [SerializeField] private BossEnemy enemy;
        [SerializeField] private AudioClip bossMusic;
        [SerializeField] private float triggerDelay = 3f;

        [SerializeField] private Teleporter teleporter;

        [SerializeField] private bool isBossDefeated = false;
        [SerializeField] private bool isEnded = false;

        [SerializeField] private bool isTriggered = false;

        private SphereCollider triggerCollider;

        private void Awake()
        {
            triggerCollider = GetComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
        }

        private IEnumerator TeleportBack()
        {

            isEnded = true;

            yield return StartCoroutine(teleporter.TeleportBack());

            isEnded = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag != "Player" || isTriggered)
                return;

            isTriggered = true;
            StartCoroutine(TriggerBossFight());
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.transform.tag != "Player" || isEnded || enemy.IsAlive)
                return;

            if (enemy.IsBossFightTriggered && !enemy.IsAlive)
            {
                isBossDefeated = true;
            }

            TipsControl.Instance.PopUp("Z", "Teleport");

            if (isBossDefeated && InputManager.Instance.InputSchemes.PlayerActions.Teleport.WasPressedThisFrame())
            {
                StartCoroutine(TeleportBack());
                TipsControl.Instance.PopOff();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.tag != "Player" || !isTriggered)
                return;

            TipsControl.Instance.PopOff();

            isTriggered = false;
            isEnded = false;
            //IngameAudioControl.Instance.Play(bossMusic);
            enemy.gameObject.GetComponentInChildren<BossInformation>().Display(false);
        }

        private IEnumerator TriggerBossFight()
        {
            BgmControl.Instance.SmoothStopMusic();

            yield return new WaitForSeconds(triggerDelay);

            enemy.TriggerBossFight();

            while (!enemy.IsBossFightTriggered)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            BgmControl.Instance.Play(bossMusic);
            enemy.gameObject.GetComponentInChildren<BossInformation>().Display(true);
        }

        public IEnumerator TriggerBossFight(float delay)
        {
            BgmControl.Instance.SmoothStopMusic();

            yield return new WaitForSeconds(delay);

            enemy.TriggerBossFight();

            while (!enemy.IsBossFightTriggered)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            BgmControl.Instance.Play(bossMusic);
            enemy.gameObject.GetComponentInChildren<BossInformation>().Display(true);
        }

    }
}
