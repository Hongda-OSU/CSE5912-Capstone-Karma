using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class BlackSoulPoint : MonoBehaviour
    {
        [SerializeField] private float experience;
        [SerializeField] private GameObject pickUpVfxPrefab;
        [SerializeField] private AudioSource pickUpAudio;

        public float statUp;
        public float duration;

        private void Awake()
        {
            experience = PlayerStats.Instance.Experience;
            PlayerStats.Instance.Experience = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            //pickUpAudio.Play();
            
            PlayerStats.Instance.Experience += experience;
            experience = 0f;
            Destroy(gameObject);

            GameObject vfx = Instantiate(pickUpVfxPrefab);
            vfx.transform.position = transform.position;
            Destroy(vfx, 10f);

            StartCoroutine(TriggerBuff());
        }

        private IEnumerator TriggerBuff()
        {
            PlayerStats.Instance.Health += statUp;
            PlayerStats.Instance.MaxHealth += statUp;
            PlayerStats.Instance.CritRate += statUp;
            PlayerStats.Instance.CritDamageFactor += statUp;

            yield return new WaitForSeconds(duration);

            PlayerStats.Instance.Health -= statUp;
            PlayerStats.Instance.MaxHealth -= statUp;
            PlayerStats.Instance.CritRate -= statUp;
            PlayerStats.Instance.CritDamageFactor -= statUp;

        }

    }
}
