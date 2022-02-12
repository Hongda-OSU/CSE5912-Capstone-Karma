using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CSE5912.PolyGamers
{
    public class BlurryController : MonoBehaviour
    {
        [SerializeField] public Material material;

        [SerializeField] private float fadingTime = 0.15f;

        [SerializeField] private float initial = 0f;
        [SerializeField] private float target = 5f;

        private string blur = "Blur";
        private float delta;

        private void Awake()
        {
            if (material == null)
                material = GetComponent<Image>().material;

            delta = target - initial;

            material.SetFloat(blur, initial);
        }

        public void SetBlurry(bool isActive)
        {
            StartCoroutine(Fade(isActive));
        }

        private IEnumerator Fade(bool isActive)
        {
            float timeSince = 0f;

            float delta = target - initial;
            if (!isActive)
                delta = -delta;

            while (timeSince < fadingTime)
            {
                float percentage = timeSince / fadingTime;

                material.SetFloat(blur, initial + delta * percentage);

                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}
