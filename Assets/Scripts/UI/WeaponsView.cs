using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class WeaponsView : UI
    {
        private List<Weapon> weaponList;

        private VisualElement slot;
        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            slot = root.Q<VisualElement>("Slot_0");
            VisualElement weaponSlot = slot.Q<VisualElement>("Weapon");
            weaponSlot.style.backgroundImage = new StyleBackground(weaponList[0].iconImage);
            weaponSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        public void UpdateWeaponList(List<Weapon> weaponList)
        {
            this.weaponList = weaponList;
        }
    }
}
