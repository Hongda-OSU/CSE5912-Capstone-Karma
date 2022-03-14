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
        private SphereCollider triggerCollider;

        private void Awake()
        {
            triggerCollider = gameObject.AddComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = areaRadius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag != "Player")
                return;

            TriggerBossFight();
        }

        private void TriggerBossFight()
        {
            enemy.TriggerBossFight();

            enemy.gameObject.GetComponentInChildren<BossInformation>().DisplayHealthBar(true);

            IngameAudioControl.Instance.TransitionToMusic(bossMusic);
        }
    }
}
