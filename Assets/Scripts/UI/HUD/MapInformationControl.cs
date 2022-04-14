using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    public class MapInformationControl : UI
    {
        [SerializeField] private float displayTime = 3f;

        [SerializeField] private int previousSceneIndex = 0;
        [SerializeField] private int currentSceneIndex = 0;

        [SerializeField] private bool isDisplayed = false;

        private VisualElement mapInformation;
        private Label mapName;

        private static MapInformationControl instance;
        public static MapInformationControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            mapInformation = root.Q<VisualElement>("MapInformation");
            mapName = mapInformation.Q<Label>("MapName");

            mapInformation.style.display = DisplayStyle.None;
            mapInformation.style.opacity = 0f;
        }

        private void Update()
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex != previousSceneIndex)
            {
                isDisplayed = false;
                StartCoroutine(DisplayMapName());
                previousSceneIndex = currentSceneIndex;
            }
        }

        public IEnumerator DisplayMapName()
        {
            if (isDisplayed)
                yield break;

            isDisplayed = true;

            yield return new WaitForSeconds(2f);

            mapName.text = SceneManager.GetActiveScene().name;

            yield return StartCoroutine(FadeIn(mapInformation));

            yield return new WaitForSeconds(displayTime);

            yield return StartCoroutine(FadeOut(mapInformation));
        }
    }
}
