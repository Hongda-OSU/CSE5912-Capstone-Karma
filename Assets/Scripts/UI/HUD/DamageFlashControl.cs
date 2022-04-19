using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class DamageFlashControl : UI
    {
        [SerializeField] private Sprite flash;
        [SerializeField] private Color color;
        [SerializeField] private bool isTriggered;

        private VisualElement panel;

        private static DamageFlashControl instance;
        public static DamageFlashControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            Initialize();

            panel = root.Q<VisualElement>("DamageFlash");

            panel.style.backgroundImage = new StyleBackground(flash);
            panel.style.unityBackgroundImageTintColor = color;
            panel.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;

            panel.style.display = DisplayStyle.Flex;
            panel.style.opacity = 0f;
        }

        private void Update()
        {
            if (isTriggered)
            {
                panel.style.display = DisplayStyle.Flex;
                panel.style.opacity = 1f;
                panel.style.unityBackgroundImageTintColor = color;
                isTriggered = false;
            }
            else
            {
                panel.style.opacity = Mathf.Lerp(panel.resolvedStyle.opacity, 0f, Time.deltaTime / fadingTime);
            }
        }

        public void TriggerDamageFlash()
        {
            isTriggered = true;
        }
    }
}
