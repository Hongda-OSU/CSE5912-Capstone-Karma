using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class QuantumBreak : PlayerSkill
    {
        [Header("")]
        [SerializeField] private float triggerHealthPercentage = 0.3f;

        [SerializeField] private float duration = 8f;
        [SerializeField] private float slowTimeScale = 0.2f;
        [SerializeField] private float fadingTime = 2f;

        [SerializeField] private RadiaBlur radiaBlur;
        [SerializeField] private float radiaLevel = 15;
        private float initRadiaLevel;

        [SerializeField] private ColorAdjustEffect cae;
        [SerializeField] private float caeSaturation = 0.5f;
        private float initCaeSaturation;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clipIn;
        [SerializeField] private AudioClip clipOut;

        private void Awake()
        {
            initRadiaLevel = radiaBlur.Level;
            initCaeSaturation = cae.saturation;
        }
        private void Update()
        {
            if (PlayerStats.Instance.Health <= PlayerStats.Instance.MaxHealth * triggerHealthPercentage)
            {
                StartCoroutine(Perform());
            }
        }
        private IEnumerator Perform()
        {
            if (!isReady)
                yield break;
            isReady = false;

            audioSource.PlayOneShot(clipIn);

            float timeSince = 0f;
            while (timeSince < fadingTime)
            {
                var t = timeSince / fadingTime;

                Time.timeScale = Mathf.Lerp(Time.timeScale, slowTimeScale, t);
                radiaBlur.Level = Mathf.Lerp(radiaBlur.Level, radiaLevel, t);
                cae.saturation = Mathf.Lerp(cae.saturation, caeSaturation, t);

                timeSince += Time.unscaledDeltaTime;
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }

            yield return new WaitForSecondsRealtime(duration);

            audioSource.PlayOneShot(clipOut);

            radiaBlur.Level = Mathf.Lerp(radiaBlur.Level, initRadiaLevel, 2);
            timeSince = 0f;
            while (timeSince < fadingTime)
            {
                var t = timeSince / fadingTime;

                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, t);
                cae.saturation = Mathf.Lerp(cae.saturation, initCaeSaturation, t);

                timeSince += Time.unscaledDeltaTime;
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }

            StartCoolingdown();
        }

    }
}
