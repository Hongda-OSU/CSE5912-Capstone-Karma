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
        public VisualElement Root
        {
            get { return root; }
        }
        protected VisualElement background;

        protected List<Button> buttonList;


        // set time interval when switching UIs
        public void SetFadingTime(float fadingTime)
        {
            this.fadingTime = fadingTime;
        }

        // initialize for all UIs
        protected void Initialize()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            background = root.Q<VisualElement>("Background");

            buttonList = new List<Button>();
            root.Query<Button>().ForEach(button => buttonList.Add(button));
        }

        // set interactability of buttons
        // mainly used to prevent clicking by mistake when switching UIs
        protected void SetButtonsInteractable(bool isInteractable)
        {
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
        protected void LoadUI(UI from, UI to)
        {
            StartCoroutine(FadeTo(from, to));
        }
        


        /*
         *  animation related
         */

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

        private IEnumerator FadeTo(UI from, UI to)
        {
            yield return StartCoroutine(from.FadeOut());

            to.background.style.opacity = 1f;
            foreach (VisualElement child in to.background.Children())
                child.style.opacity = 0f;

            yield return StartCoroutine(to.FadeIn());
        }

    }
}
