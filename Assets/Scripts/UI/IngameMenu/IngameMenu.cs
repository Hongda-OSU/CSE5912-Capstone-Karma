using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    public class IngameMenu : UI
    {
        [SerializeField] private AudioSource switchSound;

        private WeaponsPanelControl weaponsPanelControl;
        private StatsPanelControl statsPanelControl;
        private SkillsPanelControl skillsPanelControl;

        private VisualElement currentPanel;

        private Toggle weaponsToggle;
        private Toggle statsToggle;
        private Toggle skillsToggle;
        private List<Toggle> toggleList;

        private VisualElement weaponsPanel;
        private VisualElement statsPanel;
        private VisualElement skillsPanel;

        private bool isFadingComplete = true;

        private static IngameMenu instance;
        public static IngameMenu Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            Initialize();
        }
        private void Start()
        {
            weaponsPanelControl = WeaponsPanelControl.Instance;
            skillsPanelControl = SkillsPanelControl.Instance;

            toggleList = new List<Toggle>();

            weaponsPanel = root.Q<VisualElement>("WeaponsPanel");
            statsPanel = root.Q<VisualElement>("StatsPanel");
            skillsPanel = root.Q<VisualElement>("SkillsPanel");

            // set up toggles
            weaponsToggle = root.Q<Toggle>("Weapons");
            weaponsToggle.RegisterValueChangedCallback(evt => WeaponsToggleOnChange());
            toggleList.Add(weaponsToggle);

            statsToggle = root.Q<Toggle>("Stats");
            statsToggle.RegisterValueChangedCallback(evt => ModsToggleOnChange());
            toggleList.Add(statsToggle);

            skillsToggle = root.Q<Toggle>("Skills");
            skillsToggle.RegisterValueChangedCallback(evt => SkillsToggleOnChange());
            toggleList.Add(skillsToggle);

            // set default view to loadout
            currentPanel = weaponsPanel;
            weaponsPanel.style.display = DisplayStyle.Flex;
            weaponsToggle.value = true;

            // hide other views
            statsPanel.style.display = DisplayStyle.None;
            skillsPanel.style.display = DisplayStyle.None;

            root.style.display = DisplayStyle.None;
            
        }

        public IEnumerator DisplayMenu(bool enabled)
        {
            if (!isFadingComplete)
                yield break;

            isFadingComplete = false;
            if (!enabled)
            {
                yield return StartCoroutine(FadeOut(root));
            }
            else
            {
                yield return StartCoroutine(FadeIn(root));
            }
            isFadingComplete = true;
        }

        // switch to weapons view
        private void WeaponsToggleOnChange()
        {
            if (weaponsToggle.value && currentPanel != weaponsPanel)
            {
                StartCoroutine(FadeTo(currentPanel, weaponsPanel));
                currentPanel = weaponsPanel;
                UpdateToggles(weaponsToggle);

                weaponsPanelControl.ResetPanel();
            }
            else if (!weaponsToggle.value && currentPanel == weaponsPanel)
                weaponsToggle.value = true;

            switchSound.Play();
        }

        // switch to mods view
        private void ModsToggleOnChange()
        {
            if (statsToggle.value && currentPanel != statsPanel)
            {
                StartCoroutine(FadeTo(currentPanel, statsPanel));
                currentPanel = statsPanel;
                UpdateToggles(statsToggle);
            }
            else if (!statsToggle.value && currentPanel == statsPanel)
                statsToggle.value = true;

            switchSound.Play();
        }

        // switch to skills view
        private void SkillsToggleOnChange()
        {
            if (skillsToggle.value && currentPanel != skillsPanel)
            {
                StartCoroutine(FadeTo(currentPanel, skillsPanel));
                currentPanel = skillsPanel;
                UpdateToggles(skillsToggle);

                SkillsPanelControl.Instance.ResetPanel();
            }
            else if (!skillsToggle.value && currentPanel == skillsPanel)
                skillsToggle.value = true;

            switchSound.Play();
        }


        // turn other toggles off
        private void UpdateToggles(Toggle target)
        {
            foreach (Toggle toggle in toggleList)
            {
                if (toggle != target)
                {
                    toggle.value = false;
                }
            }
        }


        public bool IsFadingComplete { get { return isFadingComplete; } }
    }
}
