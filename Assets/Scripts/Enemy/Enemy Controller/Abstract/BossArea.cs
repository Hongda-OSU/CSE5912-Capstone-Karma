using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class BossArea : MonoBehaviour
    {
        [SerializeField] private float areaRadius = 20f;
        [SerializeField] private BossEnemy enemy;
        [SerializeField] private AudioClip bossMusic;
        [SerializeField] private float triggerDelay = 3f;

        private bool isTriggered = false;
        private SphereCollider triggerCollider;

        private void Awake()
        {
            triggerCollider = gameObject.AddComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = areaRadius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag != "Player" || isTriggered)
                return;

            isTriggered = true;
            StartCoroutine(TriggerBossFight());
        }

        private IEnumerator TriggerBossFight()
        {
            IngameAudioControl.Instance.SmoothMusicVolume(0f);

            yield return new WaitForSeconds(triggerDelay);

            enemy.TriggerBossFight();

            while (!enemy.IsBossFightTriggered)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            IngameAudioControl.Instance.Play(bossMusic);
            enemy.gameObject.GetComponentInChildren<BossInformation>().DisplayHealthBar(true);
        }


    }
}
