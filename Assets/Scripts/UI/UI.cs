using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class UI : MonoBehaviour
    {
        private float fadingTime = 0.3f;
        float delta = 0.01f;

        protected VisualElement root;
        protected VisualElement background;

        public void SetFadingTime(float fadingTime)
        {
            this.fadingTime = fadingTime;
        }

        protected void Initialize()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            background = root.Q<VisualElement>("Background");
        }

        private IEnumerator FadeIn()
        {
            float time = 0f;

            root.style.display = DisplayStyle.Flex;
            while (time < fadingTime)
            {
                time += delta;
                yield return new WaitForSecondsRealtime(delta);

                foreach (VisualElement child in background.Children())
                {
                    child.style.opacity = time / fadingTime;
                }
            }
        }

        private IEnumerator FadeOut()
        {
            float time = 0f;

            while (time < fadingTime)
            {
                time += delta;
                yield return new WaitForSecondsRealtime(delta);

                foreach (VisualElement child in background.Children())
                {
                    child.style.opacity = 1 - time / fadingTime;
                }
            }
            root.style.display = DisplayStyle.None;
        }

        protected IEnumerator LoadUI(GameObject target)
        {
            yield return StartCoroutine(FadeOut());
            Debug.Log(fadingTime);
            target.GetComponent<UI>().background.style.opacity = 1f;
            foreach (VisualElement child in target.GetComponent<UI>().background.Children())
                child.style.opacity = 0f;

            yield return StartCoroutine(target.GetComponent<UI>().FadeIn());
        }
    }
}
