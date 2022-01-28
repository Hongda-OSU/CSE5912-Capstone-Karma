using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class WeaponsView : UI
    {
        private List<VisualElement> slotList;
        private Dictionary<VisualElement, Weapon> slotToWeapon;
        private VisualElement specific;

        // test
        InputActions inputActions;
        private void Awake()
        {
            Initialize();

            slotList = new List<VisualElement>();
            for (int i = 0; i < root.Q<VisualElement>("Slots").childCount; i++)
            {
                slotList.Add(Root.Q<VisualElement>("Slot_" + i));
            }

            slotToWeapon = new Dictionary<VisualElement, Weapon>();
            for (int i = 0; i < slotList.Count; i++)
            {
                VisualElement slot = slotList[i];
                slotToWeapon.Add(slot, null);

                slot.RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopUpSpecific(slot)));
            }

            specific = root.Q<VisualElement>("Specific");
            specific.style.display = DisplayStyle.None;
            

            // test
            inputActions = new InputActions();
            inputActions.UI.Enable();
        }

        private void Start()
        {
            
        }

        private void Update()
        {
        }

        public void ResetView()
        {
            specific.style.display = DisplayStyle.None;
        }

        private void SetWeaponToSlot(VisualElement slot, Weapon weapon)
        {
            VisualElement weaponSlot = slot.Q<VisualElement>("Weapon");
            weaponSlot.style.backgroundImage = new StyleBackground(weapon.iconImage);
            weaponSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

            slotToWeapon[slot] = weapon;
        }

        private IEnumerator PopUpSpecific(VisualElement slot)
        {
            yield return StartCoroutine(FadeOut(specific));

            Weapon weapon = slotToWeapon[slot];

            specific.Q<Label>("Description").text = weapon.description;

            yield return StartCoroutine(FadeIn(specific));

        }



        public void UpdateWeaponList(Weapon[] weapons)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    SetWeaponToSlot(slotList[i], weapons[i]);
                }
            }

        }
    }
}
