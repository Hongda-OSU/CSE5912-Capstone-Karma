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

            public Profile(Button profile)
            {
                button = profile;

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
            }

            public void LoadPreview(int index)
            {
                var saveData = DataManager.Instance.Load(index);
                if (saveData != null)
                {
                    playerData.style.display = DisplayStyle.Flex;
                    gameData.style.display = DisplayStyle.Flex;
                    newGame.style.display = DisplayStyle.None;

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
                }
            }
        }
        private VisualElement panel;

        private List<Profile> profileList = new List<Profile>();

        private Button back;


        private static ProfilesPanelControl instance;
        public static ProfilesPanelControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            panel = root.Q<VisualElement>("ProfilesPanel");

            foreach (Button child in panel.Q<VisualElement>("Profiles").Children())
            {
                profileList.Add(new Profile(child));
            }

            back = panel.Q<Button>("Back");
        }

        private void Start()
        {
            for (int i = 0; i < profileList.Count; i++)
            {
                profileList[i].AssignAction(i);
                profileList[i].LoadPreview(i);
            }

            back.clicked += BackButtonPressed;
        }

        private void ProfileButtonPressed(int index)
        {
            var saveData = DataManager.Instance.Load(index);

            int sceneIndex = saveData == null ? 1 : saveData.sceneIndex;

            StartCoroutine(FadeOutBgm());

            SceneLoader.Instance.LoadLevel(sceneIndex);
            //DataManager.Instance.Load();

            StartSceneMenu.Instance.clickSound.Play();
            root.style.display = DisplayStyle.None;
        }


        private void BackButtonPressed()
        {
            StartCoroutine(FadeTo(panel, root.Q<VisualElement>("MainMenuPanel")));
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
