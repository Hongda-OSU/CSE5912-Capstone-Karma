using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class BossArea : MonoBehaviour
    {
        [SerializeField] private float areaRadius = 20f;
        [SerializeField] private BossInformation bossInformation;
        [SerializeField] private AudioClip bossMusic;
        private SphereCollider triggerCollider;

        private void Awake()
        {
            triggerCollider = GetComponent<SphereCollider>();
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
            bossInformation.DisplayHealthBar(true);

            IngameAudioControl.Instance.TransitionToMusic(bossMusic);
        }
    }
}
