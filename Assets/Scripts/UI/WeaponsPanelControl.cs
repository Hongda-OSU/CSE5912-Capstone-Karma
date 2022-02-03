using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class WeaponsPanelControl : UI
    {
        private List<VisualElement> slotList;
        private Dictionary<VisualElement, Weapon> slotToWeapon;
        //private Dictionary<VisualElement, AddOn[]> slotToAddOns;

        private VisualElement specificPanel;
        private VisualElement addOnsPanel;

        private VisualElement selectedWeaponSlot;
        private VisualElement selectedAddOnSlot;

        private float height;
        private Vector2 prevPosition;

        // test
        InputActions inputActions;
        private void Awake()
        {
            Initialize();

            VisualElement slots = root.Q<VisualElement>("Slots");
            slotList = new List<VisualElement>();
            for (int i = 0; i < slots.childCount; i++)
            {
                slotList.Add(Root.Q<VisualElement>("Slot_" + i));
            }

            slotToWeapon = new Dictionary<VisualElement, Weapon>();
            for (int i = 0; i < slotList.Count; i++)
            {
                VisualElement slot = slotList[i];
                slotToWeapon.Add(slot, null);

                slot.Q<VisualElement>("Weapon").RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopUpWeaponSpecific(slot)));

                // test for add-on slots on click
                slot.Q<VisualElement>("AddOn_0").RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopUpAddOns(slot)));
            }

            specificPanel = root.Q<VisualElement>("Specific");
            specificPanel.style.display = DisplayStyle.None;
            specificPanel.RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopOffWeaponSpecific()));

            addOnsPanel = root.Q<VisualElement>("AddOns");
            addOnsPanel.style.display = DisplayStyle.None;
            addOnsPanel.RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopOffAddOns()));

            // test
            slots.RegisterCallback<GeometryChangedEvent>(evt => height = slots.resolvedStyle.height / slots.childCount);
        }


        public void ResetView()
        {
            StartCoroutine(PopOffAddOns());
            StartCoroutine(PopUpWeaponSpecific(selectedWeaponSlot));
        }

        private void SetWeaponToSlot(VisualElement slot, Weapon weapon)
        {
            VisualElement weaponSlot = slot.Q<VisualElement>("Weapon");
            weaponSlot.style.backgroundImage = new StyleBackground(weapon.iconImage);
            weaponSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

            slotToWeapon[slot] = weapon;

            // todo set weapon addons to addon slots
        }

        private IEnumerator PopUpWeaponSpecific(VisualElement weaponSlot)
        {
            if (selectedWeaponSlot != weaponSlot)
            {
                selectedWeaponSlot = weaponSlot;

                yield return StartCoroutine(FadeOut(specificPanel));

                Weapon weapon = slotToWeapon[weaponSlot];

                specificPanel.Q<Label>("Description").text = weapon.description;

                yield return StartCoroutine(FadeIn(specificPanel));
            }
        }

        private IEnumerator PopOffWeaponSpecific()
        {
            if (selectedWeaponSlot != null)
            {
                StartCoroutine(FadeOut(specificPanel));

                selectedWeaponSlot = null;
            }

            yield return null;
        }

        private IEnumerator PopUpAddOns(VisualElement addOnSlot)
        {
            if (selectedAddOnSlot != addOnSlot)
            {
                prevPosition.x = addOnSlot.resolvedStyle.top;
                prevPosition.y = addOnSlot.resolvedStyle.left;

                selectedAddOnSlot = addOnSlot;

                StartCoroutine(TranslateTo(addOnSlot, 0f, 0f));
                StartCoroutine(FadeIn(addOnsPanel));

                foreach (VisualElement slot in slotList)
                {
                    if (slot != addOnSlot)
                        StartCoroutine(FadeOut(slot));
                }

                // todo - change specific to selected add-on
            }
            yield return null;
        }

        private IEnumerator PopOffAddOns()
        {
            if (selectedAddOnSlot != null)
            {
                StartCoroutine(TranslateTo(selectedAddOnSlot, prevPosition.x, prevPosition.y));
                StartCoroutine(FadeOut(addOnsPanel));

                foreach (VisualElement slot in slotList)
                {
                    if (slot != selectedAddOnSlot)
                        StartCoroutine(FadeIn(slot));
                }
                selectedAddOnSlot = null;
            }
            yield return null;
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
