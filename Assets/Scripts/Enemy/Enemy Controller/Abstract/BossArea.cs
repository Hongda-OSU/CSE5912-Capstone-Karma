using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    public class BossArea : MonoBehaviour
    {
        public bool isPlayerInside = false;

        [SerializeField] private bool allowReturn = true;

        [SerializeField] private BossEnemy enemy;
        public AudioClip bossMusic;
        [SerializeField] private float triggerDelay = 3f;

        public Teleporter teleporter;

        public bool isBossDefeated = false;
        [SerializeField] private bool isEnded = false;

        [SerializeField] private bool isTriggered = false;

        private Collider triggerCollider;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
        }

        private IEnumerator TeleportBack()
        {
            isEnded = true;

            yield return StartCoroutine(teleporter.TeleportBack());
            BgmControl.Instance.PlayCurrentBgm();

            isEnded = false;
            isPlayerInside = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag != "Player" || isTriggered)
                return;

            isTriggered = true;
            isPlayerInside = true;
            StartCoroutine(TriggerBossFight());
        }

        private void Update()
        {

            if (isPlayerInside && isBossDefeated && InputManager.Instance.InputSchemes.PlayerActions.Teleport.WasPressedThisFrame())
            {
                StartCoroutine(TeleportBack());
                TipsControl.Instance.PopOff();
                DataManager.Instance.Save();
            }
            else if (!isPlayerInside)
            {
                isTriggered = false;
                isEnded = false;
                enemy.gameObject.GetComponentInChildren<BossInformation>().Display(false);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.transform.tag != "Player" || isEnded || enemy.IsAlive)
                return;

            if (enemy.IsBossFightTriggered && !enemy.IsAlive)
            {
                SetBossDefeated(true);
            }

            isPlayerInside = true;

            if (!allowReturn)
                return;

            TipsControl.Instance.PopUp("Z", "Teleport");

        }

        public void SetBossDefeated(bool isDefeated)
        {
            if (isBossDefeated == isDefeated)
                return;

            isBossDefeated = isDefeated;

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.tag != "Player" || !isTriggered)
                return;

            TipsControl.Instance.PopOff();
            isPlayerInside = false;

        }

        private IEnumerator TriggerBossFight()
        {
            Debug.Log("Boss Fight Triggered. " + enemy.EnemyName);
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


        public bool GetIsTriggered() 
        {
            return isTriggered;
        }

        public bool GetIsBossDefeated()
        {
            return isBossDefeated;
        }
    }
}
