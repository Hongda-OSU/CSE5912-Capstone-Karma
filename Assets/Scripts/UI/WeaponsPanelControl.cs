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
        private Dictionary<VisualElement, Firearms> slotToWeapon;
        //private Dictionary<VisualElement, AddOn[]> slotToAddOns;

        private VisualElement specificPanel;
        private VisualElement attachmentsPanel;

        private VisualElement selectedWeaponSlot;
        private VisualElement selectedAttachmentSlot;

        private float height;
        private Vector2 prevPosition;

        // test

        private void Awake()
        {
            Initialize();

            VisualElement slots = root.Q<VisualElement>("Slots");
            slotList = new List<VisualElement>();
            for (int i = 0; i < slots.childCount; i++)
            {
                slotList.Add(Root.Q<VisualElement>("Slot_" + i));
            }

            slotToWeapon = new Dictionary<VisualElement, Firearms>();
            for (int i = 0; i < slotList.Count; i++)
            {
                VisualElement slot = slotList[i];
                slotToWeapon.Add(slot, null);

                slot.Q<VisualElement>("Weapon").RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopUpWeaponSpecific(slot)));

                // test for add-on slots on click
                slot.Q<VisualElement>("Attachment_0").RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopUpAttachments(slot)));
            }

            specificPanel = root.Q<VisualElement>("Specific");
            specificPanel.style.display = DisplayStyle.None;
            specificPanel.RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopOffWeaponSpecific()));

            attachmentsPanel = root.Q<VisualElement>("Attachments");
            attachmentsPanel.style.display = DisplayStyle.None;
            attachmentsPanel.RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopOffAttachments()));

            // test
            slots.RegisterCallback<GeometryChangedEvent>(evt => height = slots.resolvedStyle.height / slots.childCount);
        }


        public void ResetView()
        {
            StartCoroutine(PopOffAttachments());
            StartCoroutine(PopUpWeaponSpecific(selectedWeaponSlot));
        }

        private void SetWeaponToSlot(VisualElement slot, Firearms weapon)
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

                Firearms weapon = slotToWeapon[weaponSlot];

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

        private IEnumerator PopUpAttachments(VisualElement attachmentSlot)
        {
            if (selectedAttachmentSlot != attachmentSlot)
            {
                prevPosition.x = attachmentSlot.resolvedStyle.top;
                prevPosition.y = attachmentSlot.resolvedStyle.left;

                selectedAttachmentSlot = attachmentSlot;

                StartCoroutine(TranslateTo(attachmentSlot, 0f, 0f));
                StartCoroutine(FadeIn(attachmentsPanel));

                foreach (VisualElement slot in slotList)
                {
                    if (slot != attachmentSlot)
                        StartCoroutine(FadeOut(slot));
                }

                // todo - change specific to selected add-on
            }
            yield return null;
        }

        private IEnumerator PopOffAttachments()
        {
            if (selectedAttachmentSlot != null)
            {
                StartCoroutine(TranslateTo(selectedAttachmentSlot, prevPosition.x, prevPosition.y));
                StartCoroutine(FadeOut(attachmentsPanel));

                foreach (VisualElement slot in slotList)
                {
                    if (slot != selectedAttachmentSlot)
                        StartCoroutine(FadeIn(slot));
                }
                selectedAttachmentSlot = null;
            }
            yield return null;
        }

        public void UpdateWeaponList(Firearms[] weapons)
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
