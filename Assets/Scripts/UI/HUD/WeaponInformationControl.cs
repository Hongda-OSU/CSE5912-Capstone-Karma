using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class WeaponInformationControl : UI
    {
        [SerializeField] private Firearms currentWeapon;
        [SerializeField] private Color backgroundColor;

        private VisualElement weaponInformation;
        private List<VisualElement> weaponInformationList;
        private List<Firearms> playerWeaponList;

        private VisualElement prevWeapon;
        private int prevAmmo;

        private static WeaponInformationControl instance;
        public static WeaponInformationControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
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
            UpdateWeapon();
            UpdateInformation();
        }

        public void UpdateWeapon()
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

                    if (WeaponManager.Instance.CarriedWeapon == playerWeaponList[i])
                    {
                        weapon.style.backgroundColor = backgroundColor;
                    }
                    else
                    {
                        weapon.style.backgroundColor = Color.clear;
                    }
                }
                else
                {
                    weapon.style.display = DisplayStyle.None;
                }
            }
        }

        public void UpdateInformation()
        {
            currentWeapon = WeaponManager.Instance.CarriedWeapon;
            int index = playerWeaponList.IndexOf(currentWeapon);

            for (int i = 0; i < weaponInformationList.Count; i++)
            {
                var weapon = weaponInformationList[i];

                if (i == index)
                {
                    weapon.style.opacity = 1f;

                    int ammo = currentWeapon.CurrentAmmo;
                    int maxAmmo = currentWeapon.GetCurrentMaxAmmo;
                    weapon.Q<Label>("Ammo").text = ammo.ToString() + " / " + maxAmmo.ToString();

                    if (prevAmmo > ammo && prevWeapon == weapon)
                    {
                        //StartCoroutine(FadeIn(weapon));
                    }
                    prevAmmo = ammo;
                    

                    prevWeapon = weapon;
                }
                else
                {
                    weapon.style.opacity = 0.5f;
                    weapon.Q<Label>("Ammo").text = "";
                }
            }
        }

        public override void Display(bool enabled)
        {
            if (enabled)
            {
                weaponInformation.style.display = DisplayStyle.Flex;
            }
            else
            {
                weaponInformation.style.display = DisplayStyle.None;
            }
        }
    }
}
