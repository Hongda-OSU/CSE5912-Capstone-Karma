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

        protected List<Button> buttonList;

        public void SetFadingTime(float fadingTime)
        {
            this.fadingTime = fadingTime;
        }

        protected void Initialize()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            background = root.Q<VisualElement>("Background");

            buttonList = new List<Button>();
            root.Query<Button>().ForEach(button => buttonList.Add(button));
        }

        protected void SetButtonsInteractable(bool isInteractable)
        {
            foreach (Button button in buttonList)
            {
                if (isInteractable)
                    button.pickingMode = PickingMode.Position;
                else
                    button.pickingMode = PickingMode.Ignore;
            }

        }
        protected void LoadUI(GameObject target)
        {
            StartCoroutine(FadeTo(target));
        }

        private IEnumerator FadeIn()
        {
            SetButtonsInteractable(false);

            root.style.display = DisplayStyle.Flex;

            float time = 0f;
            while (time < fadingTime)
            {
                time += delta;
                yield return new WaitForSecondsRealtime(delta);

                foreach (VisualElement child in background.Children())
                    child.style.opacity = time / fadingTime;
                
            }

            SetButtonsInteractable(true);
        }

        private IEnumerator FadeOut()
        {
            SetButtonsInteractable(false);

            float time = 0f;
            while (time < fadingTime)
            {
                time += delta;
                yield return new WaitForSecondsRealtime(delta);

                foreach (VisualElement child in background.Children())
                    child.style.opacity = 1 - time / fadingTime;
                
            }
            root.style.display = DisplayStyle.None;
        }

        private IEnumerator FadeTo(GameObject target)
        {

            yield return StartCoroutine(FadeOut());

            target.GetComponent<UI>().background.style.opacity = 1f;
            foreach (VisualElement child in target.GetComponent<UI>().background.Children())
                child.style.opacity = 0f;

            yield return StartCoroutine(target.GetComponent<UI>().FadeIn());
        }

    }
}
