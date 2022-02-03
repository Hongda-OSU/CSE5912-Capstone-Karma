using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class UI : MonoBehaviour
    {
        private float fadingTime = 0.3f;
        float delta = 0.01f;

        protected VisualElement root;
        public VisualElement Root
        {
            get { return root; }
        }

        // set time interval when switching UIs
        public void SetFadingTime(float fadingTime)
        {
            this.fadingTime = fadingTime;
        }

        // initialize for all UIs
        protected void Initialize()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
        }

        // set interactability of buttons
        // mainly used to prevent clicking by mistake when switching UIs
        protected void SetButtonsInteractable(VisualElement root, bool isInteractable)
        {
            List<Button> buttonList = new List<Button>();
            root.Query<Button>().ForEach(button => buttonList.Add(button));
            if (buttonList.Count > 0)
            {
                foreach (Button button in buttonList)
                {
                    if (isInteractable)
                        button.pickingMode = PickingMode.Position;
                    else
                        button.pickingMode = PickingMode.Ignore;
                }
            }
        }

        // load from current UI to another
        protected IEnumerator FadeTo(VisualElement from, VisualElement to)
        {
            yield return StartCoroutine(FadeOut(from));

            yield return StartCoroutine(FadeIn(to));
        }


        /*
         *  animation related
         */

        protected IEnumerator FadeOut(VisualElement element)
        {
            SetButtonsInteractable(element, false);

            float time = 0f;
            while (time < fadingTime)
            {
                time += delta;
                yield return new WaitForSecondsRealtime(delta);

                element.style.opacity = 1 - time / fadingTime;
            }

            element.style.opacity = 0f;
            element.style.display = DisplayStyle.None;
        }

        protected IEnumerator FadeIn(VisualElement element)
        {
            SetButtonsInteractable(element, false);

            element.style.opacity = 0f;
            element.style.display = DisplayStyle.Flex;

            float time = 0f;
            while (time < fadingTime)
            {
                time += delta;
                yield return new WaitForSecondsRealtime(delta);

                element.style.opacity = time / fadingTime;
            }

            SetButtonsInteractable(element, true);
        }


        protected IEnumerator TranslateTo(VisualElement element, float top, float left)
        {
            SetButtonsInteractable(element, false);

            float deltaTop = element.resolvedStyle.top - top;
            float deltaLeft = element.resolvedStyle.left - left;

            float time = 0f;
            while (time < fadingTime)
            {
                element.style.top = top + deltaTop * (1 - time / fadingTime);
                element.style.left = left + deltaLeft * (1 - time / fadingTime);

                time += delta;
                yield return new WaitForSecondsRealtime(delta);
            }

            SetButtonsInteractable(element, true);
        }


    }
}
