using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class ProfilesPanelControl : UI
    {
        private class Profile
        {
            public Button button;
            public Label indexLabel;
            public VisualElement playerData;
            public VisualElement gameData;
            public Label newGame;
            public Button clear;

            public Profile(Button profile, Button clear)
            {
                button = profile;
                this.clear = clear;

                indexLabel = profile.Q<Label>("Index");
                playerData = profile.Q<VisualElement>("PlayerData");
                gameData = profile.Q<VisualElement>("GameData");
                newGame = profile.Q <Label>("New");
            }

            public void AssignAction(int index)
            {
                button.clicked += delegate 
                { 
                    Instance.ProfileButtonPressed(index); 
                };

                clear.clicked += delegate
                {
                    Instance.ClearButtonPressed(index);
                };
            }

            public void LoadPreview(int index)
            {
                var saveData = DataManager.Instance.Load(index);
                if (saveData != null)
                {
                    playerData.style.display = DisplayStyle.Flex;
                    gameData.style.display = DisplayStyle.Flex;
                    newGame.style.display = DisplayStyle.None;
                    clear.style.opacity = 1f;

                    playerData.Q<Label>("Level").text = saveData.playerStatsData.level.ToString();
                    playerData.Q<Label>("Exp").text = saveData.playerStatsData.experience.ToString() + " / " + saveData.playerStatsData.experienceToUpgrade.ToString();

                    gameData.Q<Label>("Map").text = saveData.mapName;

                    int time = saveData.gamePlayTimeInMinutes;
                    var hours = time / 60;
                    var minutes = time % 60;
                    gameData.Q<Label>("Time").text = hours + "H " + minutes + "M";

                    Debug.Log(time + " = " + hours + ", " + minutes);
                }
                else
                {
                    playerData.style.display = DisplayStyle.None;
                    gameData.style.display = DisplayStyle.None;
                    newGame.style.display = DisplayStyle.Flex;
                    clear.style.opacity = 0f;
                }
            }
        }
        private VisualElement panel;
        private VisualElement shade;

        private List<Profile> profileList = new List<Profile>();

        private Button back;

        private int clearIndex = -1;

        private static ProfilesPanelControl instance;
        public static ProfilesPanelControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            panel = root.Q<VisualElement>("ProfilesPanel");

            shade = root.Q<VisualElement>("Shade");
            shade.style.display = DisplayStyle.None;

            foreach (VisualElement child in panel.Q<VisualElement>("Profiles").Children())
            {
                var profile = child.Q<Button>("Profile_" + child.name);
                var clear = child.Q<Button>("Clear");

                profileList.Add(new Profile(profile, clear));
            }

            back = panel.Q<Button>("Back");
        }

        private void Start()
        {
            for (int i = 0; i < profileList.Count; i++)
            {
                profileList[i].AssignAction(i);
            }
            LoadPreviews();

            shade.Q<Button>("Yes").clicked += YesButtonPressed;
            shade.Q<Button>("No").clicked += NoButtonPressed;

            back.clicked += BackButtonPressed;
        }

        private void ProfileButtonPressed(int index)
        {
            var saveData = DataManager.Instance.Load(index);

            int sceneIndex = saveData == null ? 1 : saveData.sceneIndex;

            StartCoroutine(FadeOutBgm());

            SceneLoader.Instance.LoadLevel(sceneIndex);
            DataManager.Instance.LoadDataToGame(saveData, index);

            StartSceneMenu.Instance.clickSound.Play();
            root.style.display = DisplayStyle.None;
        }

        private void ClearButtonPressed(int index)
        {
            clearIndex = index;
            StartCoroutine(FadeIn(shade));
        }
        private void YesButtonPressed()
        {
            if (clearIndex > -1)
            {
                DataManager.Instance.Clear(clearIndex);
                clearIndex = -1;
            }
            StartCoroutine(FadeOut(shade));
            LoadPreviews();
        }
        private void NoButtonPressed()
        {
            clearIndex = -1;
            StartCoroutine(FadeOut(shade));
        }

        private void BackButtonPressed()
        {
            StartCoroutine(FadeTo(panel, root.Q<VisualElement>("MainMenuPanel")));
        }


        private void LoadPreviews()
        {
            for (int i = 0; i < profileList.Count; i++)
            {
                profileList[i].LoadPreview(i);
            }
        }
        private IEnumerator FadeOutBgm()
        {
            float timeSince = 0f;
            float fadeoutTime = 0.5f;
            while (timeSince < fadeoutTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                StartSceneMenu.Instance.bgm.volume = fadeoutTime - timeSince;
            }
        }
    }
}
