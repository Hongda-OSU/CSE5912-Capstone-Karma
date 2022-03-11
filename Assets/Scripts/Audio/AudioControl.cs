using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class AudioControl : MonoBehaviour
    {
        [SerializeField] private AudioSource mainAudio;

        private static AudioControl instance;
        public static AudioControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;
        }

        public void TransitionToMusic(AudioClip clip)
        {
            StartCoroutine(FadeAndPlay(clip));
        }

        private IEnumerator FadeAndPlay(AudioClip clip)
        {
            float timeSince = 0f;
            float fadeoutTime = 0.5f;
            while (timeSince < fadeoutTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                mainAudio.volume = fadeoutTime - timeSince;
            }

            mainAudio.volume = 1f;
            mainAudio.clip = clip;
            mainAudio.Play();
        }
        private void PlayerMusic(AudioClip clip)
        {
            mainAudio.volume = 1f;
            mainAudio.clip = clip;
            mainAudio.Play();
        }
        private IEnumerator FadeOutCurrentMusic()
        {
            float timeSince = 0f;
            float fadeoutTime = 0.5f;
            while (timeSince < fadeoutTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                mainAudio.volume = fadeoutTime - timeSince;
            }
        }
    }
}
