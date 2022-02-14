using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CSE5912.PolyGamers
{
    public class DepthOfFieldControl : MonoBehaviour
    {
        [SerializeField] private PostProcessVolume volume;

        private DepthOfField depthOfField;

        [SerializeField] private float fadingTime = 0.15f;

        [Header("Initial Values")]
        [SerializeField] private float initialFocusDistance = 10f;
        [SerializeField] private float initialAperture = 5.6f;
        [SerializeField] private float initialFocusLength = 50f;

        [Header("Target Values")]
        [SerializeField] private float targetFocusDistance;
        [SerializeField] private float targetAperture;
        [SerializeField] private float targetFocusLength;

        private float deltaFocusDistance;
        private float deltaAperture;
        private float deltaFocusLength;


        private void Awake()
        {
            volume.profile.TryGetSettings(out depthOfField);

            depthOfField.focusDistance.value = initialFocusDistance;
            depthOfField.aperture.value = initialAperture;
            depthOfField.focalLength.value = initialFocusLength;

            deltaFocusDistance = targetFocusDistance - initialFocusDistance;
            deltaAperture = targetAperture - initialAperture;
            deltaFocusLength = targetFocusLength - initialFocusLength;
        }

        public IEnumerator FadeIn()
        {
            float timeSince = 0f;

            while (timeSince < fadingTime)
            {
                float percentage = timeSince / fadingTime;

                depthOfField.focusDistance.value = initialFocusDistance + deltaFocusDistance * percentage;
                depthOfField.aperture.value = initialAperture + deltaAperture * percentage;
                depthOfField.focalLength.value = initialFocusLength + deltaFocusLength * percentage;

                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        public IEnumerator FadeOut()
        {
            float timeSince = 0f;

            while (timeSince < fadingTime)
            {
                float percentage = timeSince / fadingTime;

                depthOfField.focusDistance.value = targetFocusDistance - deltaFocusDistance * percentage;
                depthOfField.aperture.value = targetAperture - deltaAperture * percentage;
                depthOfField.focalLength.value = targetFocusLength - deltaFocusLength * percentage;

                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}
