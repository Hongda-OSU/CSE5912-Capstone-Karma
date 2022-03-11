using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class BossArea : MonoBehaviour
    {
        [SerializeField] private BossInformation bossInformation;
        [SerializeField] private AudioClip bossMusic;
        private Collider areaTrigger;

        private void Awake()
        {
            areaTrigger = GetComponent<Collider>();
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

            AudioControl.Instance.TransitionToMusic(bossMusic);
        }
    }
}
