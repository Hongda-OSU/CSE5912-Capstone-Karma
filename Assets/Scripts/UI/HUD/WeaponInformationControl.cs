using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class WeaponInformationControl : UI
    {
        private VisualElement weaponInformation;
        private List<VisualElement> weaponList;

        private static WeaponInformationControl instance;
        public static WeaponInformationControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            Initialize();

            weaponList = new List<VisualElement>();

            weaponInformation = root.Q<VisualElement>("WeaponInformation");
            for (int i = 0; i < weaponInformation.childCount; i++)
            {
                VisualElement child = weaponInformation.Q<VisualElement>("Weapon_" + i);
                weaponList.Add(child);
                child.style.display = DisplayStyle.None;
            }
        }

        private void Start()
        {
            var playerWeaponList = PlayerInventory.Instance.GetPlayerWeaponList();
            for (int i = 0; i < weaponList.Count; i++)
            {
                if (i < playerWeaponList.Count)
                {
                    weaponList[i].style.backgroundImage = new StyleBackground(playerWeaponList[i].iconImage);
                    weaponList[i].style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                    weaponList[i].style.display = DisplayStyle.Flex;
                }
                else
                {
                    weaponList[i].style.display = DisplayStyle.None;

                }
            }
        }
    }
}
