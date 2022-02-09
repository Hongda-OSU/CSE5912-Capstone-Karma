using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class UI : MonoBehaviour
    {
        [SerializeField] protected UIDocument uiDocument;
        [SerializeField] protected float fadingTime = 0.15f;

        private float deltaTime = 0.01f;

        protected VisualElement root;
        public VisualElement Root
        {
            get { return root; }
        }

        public void SetDisplay(bool isActive)
        {
            if (!isActive)
            {
                StartCoroutine(FadeOut(root));

                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                StartCoroutine(FadeIn(root));

                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
            }
        }

        // initialize for all UIs
        protected virtual void Initialize()
        {
            root = uiDocument.rootVisualElement;
        }

        // set interactability of buttons
        // mainly used to prevent clicking by mistake when switching UIs
        protected virtual void SetButtonsInteractable(VisualElement root, bool isInteractable)
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
                time += deltaTime;
                yield return new WaitForSecondsRealtime(deltaTime);

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
                time += deltaTime;
                yield return new WaitForSecondsRealtime(deltaTime);

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

                time += deltaTime;
                yield return new WaitForSecondsRealtime(deltaTime);
            }

            SetButtonsInteractable(element, true);
        }


    }
}
