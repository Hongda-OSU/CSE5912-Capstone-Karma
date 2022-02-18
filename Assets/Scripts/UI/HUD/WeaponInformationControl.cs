using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class WeaponInformationControl : UI
    {
        [SerializeField] private Firearms currentWeapon;

        private VisualElement weaponInformation;
        private List<VisualElement> weaponInformationList;
        private List<Firearms> playerWeaponList;

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

            weaponInformationList = new List<VisualElement>();

            weaponInformation = root.Q<VisualElement>("WeaponInformation");
            for (int i = 0; i < weaponInformation.childCount; i++)
            {
                VisualElement child = weaponInformation.Q<VisualElement>("Weapon_" + i);
                weaponInformationList.Add(child);
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
            UpdateWeaponInformation();
        }

        public void UpdateWeaponInformation()
        {

            playerWeaponList = PlayerInventory.Instance.GetPlayerWeaponList();
            for (int i = 0; i < weaponInformationList.Count; i++)
            {
                var weapon = weaponInformationList[i];
                if (i < playerWeaponList.Count)
                {
                    weapon.style.backgroundImage = new StyleBackground(playerWeaponList[i].iconImage);
                    weapon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                    weapon.style.display = DisplayStyle.Flex;

                    currentWeapon = WeaponManager.Instance.CarriedWeapon;
                    int index = playerWeaponList.IndexOf(currentWeapon);
                    if (i == index)
                    {
                        weapon.style.opacity = 1f;
                        weapon.Q<Label>("Ammo").text = currentWeapon.GetCurrentAmmo.ToString() + " / " + currentWeapon.GetCurrentMaxAmmo.ToString();

                    }
                    else
                    {
                        weapon.style.opacity = 0.5f;
                        weapon.Q<Label>("Ammo").text = "";
                    }
                }
                else
                {
                    weapon.style.display = DisplayStyle.None;
                }
            }
        }
    }
}
