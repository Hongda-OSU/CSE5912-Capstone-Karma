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

        private UI currentView;

        private Toggle weaponsToggle;
        private Toggle modsToggle;
        private Toggle skillsToggle;
        private List<Toggle> toggleList;

        private void Awake()
        {
            Initialize();
        }
        private void Start()
        {
            toggleList = new List<Toggle>();

            // set up toggles
            weaponsToggle = root.Q<Toggle>("Weapons");
            weaponsToggle.RegisterValueChangedCallback(evt => WeaponsTogglePressed());
            toggleList.Add(weaponsToggle);

            modsToggle = root.Q<Toggle>("Mods");
            modsToggle.RegisterValueChangedCallback(evt => ModsTogglePressed());
            toggleList.Add(modsToggle);

            skillsToggle = root.Q<Toggle>("Skills");
            skillsToggle.RegisterValueChangedCallback(evt => SkillsTogglePressed());
            toggleList.Add(skillsToggle);

            // set default view to loadout
            currentView = weaponsView;
            weaponsToggle.value = true;

            // hide other views
            modsView.Root.style.display = DisplayStyle.None;
            skillsView.Root.style.display = DisplayStyle.None;
        }


        // switch to weapons view
        private void WeaponsTogglePressed()
        {
            if (weaponsToggle.value && currentView != weaponsView)
            {
                LoadUI(currentView, weaponsView);
                currentView = weaponsView;
                UpdateToggles(weaponsToggle);

                weaponsView.ResetView();
            }
            else if (!weaponsToggle.value && currentView == weaponsView)
                weaponsToggle.value = true;
        }

        // switch to mods view
        private void ModsTogglePressed()
        {
            if (modsToggle.value && currentView != modsView)
            {
                LoadUI(currentView, modsView);
                currentView = modsView;
                UpdateToggles(modsToggle);
            }
            else if (!modsToggle.value && currentView == modsView)
                modsToggle.value = true;
        }

        // switch to skills view
        private void SkillsTogglePressed()
        {
            if (skillsToggle.value && currentView != skillsView)
            {
                LoadUI(currentView, skillsView);
                currentView = skillsView;
                UpdateToggles(skillsToggle);
            }
            else if (!skillsToggle.value && currentView == skillsView)
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
