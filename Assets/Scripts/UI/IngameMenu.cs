using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    public class IngameMenu : UI
    {
        [SerializeField] private WeaponsView weaponsView;
        [SerializeField] private ModsView modsView;
        [SerializeField] private SkillsView skillsView;

        private VisualElement currentPanel;

        private Toggle weaponsToggle;
        private Toggle modsToggle;
        private Toggle skillsToggle;
        private List<Toggle> toggleList;

        private VisualElement weaponsPanel;
        private VisualElement modsPanel;
        private VisualElement skillsPanel;

        private void Awake()
        {
            Initialize();
        }
        private void Start()
        {
            toggleList = new List<Toggle>();

            weaponsPanel = root.Q<VisualElement>("WeaponsPanel");
            modsPanel = root.Q<VisualElement>("ModsPanel");
            skillsPanel = root.Q<VisualElement>("SkillsPanel");

            // set up toggles
            weaponsToggle = root.Q<Toggle>("Weapons");
            weaponsToggle.RegisterValueChangedCallback(evt => WeaponsToggleOnChange());
            toggleList.Add(weaponsToggle);

            modsToggle = root.Q<Toggle>("Mods");
            modsToggle.RegisterValueChangedCallback(evt => ModsToggleOnChange());
            toggleList.Add(modsToggle);

            skillsToggle = root.Q<Toggle>("Skills");
            skillsToggle.RegisterValueChangedCallback(evt => SkillsToggleOnChange());
            toggleList.Add(skillsToggle);

            // set default view to loadout
            currentPanel = weaponsPanel;
            weaponsToggle.value = true;

            // hide other views
            modsPanel.style.display = DisplayStyle.None;
            skillsPanel.style.display = DisplayStyle.None;
        }


        // switch to weapons view
        private void WeaponsToggleOnChange()
        {
            if (weaponsToggle.value && currentPanel != weaponsPanel)
            {
                StartCoroutine(FadeTo(currentPanel, weaponsPanel));
                currentPanel = weaponsPanel;
                UpdateToggles(weaponsToggle);

                weaponsView.ResetView();
            }
            else if (!weaponsToggle.value && currentPanel == weaponsPanel)
                weaponsToggle.value = true;
        }

        // switch to mods view
        private void ModsToggleOnChange()
        {
            if (modsToggle.value && currentPanel != modsPanel)
            {
                StartCoroutine(FadeTo(currentPanel, modsPanel));
                currentPanel = modsPanel;
                UpdateToggles(modsToggle);
            }
            else if (!modsToggle.value && currentPanel == modsPanel)
                modsToggle.value = true;
        }

        // switch to skills view
        private void SkillsToggleOnChange()
        {
            if (skillsToggle.value && currentPanel != skillsPanel)
            {
                StartCoroutine(FadeTo(currentPanel, skillsPanel));
                currentPanel = skillsPanel;
                UpdateToggles(skillsToggle);
            }
            else if (!skillsToggle.value && currentPanel == skillsPanel)
                skillsToggle.value = true;
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
    }
}
