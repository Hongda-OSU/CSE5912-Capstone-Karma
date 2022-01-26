using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    public class IngameMenu : UI
    {
        [SerializeField] private UI loadoutView;
        [SerializeField] private UI skillsView;
        [SerializeField] private UI questsView;

        private UI currentView;

        private Toggle loadoutToggle;
        private Toggle skillsToggle;
        private Toggle questsToggle;

        private List<Toggle> toggleList;

        private void Start()
        {
            Initialize();

            toggleList = new List<Toggle>();

            // set up toggles
            loadoutToggle = root.Q<Toggle>("Loadout");
            loadoutToggle.RegisterValueChangedCallback(evt => LoadoutTogglePressed());
            toggleList.Add(loadoutToggle);

            skillsToggle = root.Q<Toggle>("Skills");
            skillsToggle.RegisterValueChangedCallback(evt => SkillsTogglePressed());
            toggleList.Add(skillsToggle);

            questsToggle = root.Q<Toggle>("Quests");
            questsToggle.RegisterValueChangedCallback(evt => QuestsTogglePressed());
            toggleList.Add(questsToggle);

            // set default view to loadout
            currentView = loadoutView;
            loadoutToggle.value = true;

            // hide other views
            skillsView.GetRoot().style.display = DisplayStyle.None;
            //questsView.GetRoot().style.display = DisplayStyle.None;
        }


        // switch to loadout view
        private void LoadoutTogglePressed()
        {
            if (loadoutToggle.value && currentView != loadoutView)
            {
                LoadUI(currentView, loadoutView);
                currentView = loadoutView;
                UpdateToggles(loadoutToggle);
            }

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

        }

        // switch to quests view
        private void QuestsTogglePressed()
        {
            if (questsToggle.value && currentView != questsView)
            {
                LoadUI(currentView, questsView);
                currentView = questsView;
                UpdateToggles(questsToggle);
            }
        }

        // turn other toggles off
        private void UpdateToggles(Toggle target)
        {
            foreach (Toggle toggle in toggleList)
            {
                if (toggle != target)
                    toggle.value = false;
            }
        }
    }
}
