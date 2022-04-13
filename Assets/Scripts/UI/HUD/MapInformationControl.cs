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

        public IEnumerator DisplayMapName(string name)
        {
            mapName.text = name;

            yield return StartCoroutine(FadeIn(mapInformation));

            yield return new WaitForSeconds(displayTime);

            yield return StartCoroutine(FadeOut(mapInformation));
        }
    }
}
