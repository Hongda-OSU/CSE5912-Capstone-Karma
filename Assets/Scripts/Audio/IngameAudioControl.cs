using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class IngameAudioControl : MonoBehaviour
    {
        [SerializeField] private AudioSource mainAudio;
        [SerializeField] private float fadeoutTime = 0.5f;

        private static IngameAudioControl instance;
        public static IngameAudioControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;
        }

        public IEnumerator FadeOutBgm()
        {
            float timeSince = 0f;
            while (timeSince < fadeoutTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSecondsRealtime(Time.deltaTime);

                mainAudio.volume = fadeoutTime - timeSince;
            }
        }

        public void TransitionToMusic(AudioClip clip)
        {
            StartCoroutine(FadeAndPlay(clip));
        }

        private IEnumerator FadeAndPlay(AudioClip clip)
        {
            yield return StartCoroutine(FadeOutBgm());

            mainAudio.volume = 1f;
            mainAudio.clip = clip;
            mainAudio.Play();
        }


        public float FadeoutTime { get { return fadeoutTime; } }
    }
}
