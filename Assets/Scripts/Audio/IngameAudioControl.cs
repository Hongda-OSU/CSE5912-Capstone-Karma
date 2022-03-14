using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class IngameAudioControl : MonoBehaviour
    {
        [SerializeField] private AudioSource mainAudio;

        private static IngameAudioControl instance;
        public static IngameAudioControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;
        }

        public void Play(AudioClip clip)
        {
            mainAudio.volume = 1f;
            mainAudio.clip = clip;
            mainAudio.Play();
        }

        public void SmoothMusicVolume(float volume)
        {
            StartCoroutine(FadeBgm(volume));
        }

        private IEnumerator FadeBgm(float volume)
        {
            float timeSince = 0f;
            while (timeSince < SceneLoader.Instance.FadeoutTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSecondsRealtime(Time.deltaTime);

                mainAudio.volume = Mathf.Lerp(mainAudio.volume, volume, timeSince);
            }
        }

        public void TransitionToMusic(AudioClip clip)
        {
            StartCoroutine(FadeAndPlay(clip));
        }

        private IEnumerator FadeAndPlay(AudioClip clip)
        {
            yield return StartCoroutine(FadeBgm(0f));

            mainAudio.volume = 1f;
            mainAudio.clip = clip;
            mainAudio.Play();
        }

        public AudioSource MainAudio { get { return mainAudio; } }
    }
}
