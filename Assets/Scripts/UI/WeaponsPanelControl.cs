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
        private List<VisualElement> weaponRowList;
        private Dictionary<VisualElement, Firearms> rowToWeapon;

        [SerializeField] private int attachmentPerWeapon = 4;
        private Dictionary<VisualElement, VisualElement[]> weaponToAttachments;

        private List<VisualElement> attachmentInventoryRowList;
        private List<VisualElement> attachmentInventorySlotList;

        private VisualElement specificPanel;
        private VisualElement attachmentsPanel;

        private VisualElement selectedWeaponSlot;
        private VisualElement selectedAttachmentSlot;

        private float height;
        private Vector2 prevRowPosition;

        // test

        private void Awake()
        {
            Initialize();

            VisualElement weaponsPanel = root.Q<VisualElement>("WeaponsPanel");

            VisualElement weaponSlots = weaponsPanel.Q<VisualElement>("Slots");
            weaponSlots.style.display = DisplayStyle.Flex;

            weaponRowList = new List<VisualElement>();
            for (int i = 0; i < weaponSlots.childCount; i++)
            {
                weaponRowList.Add(weaponSlots.Q<VisualElement>("Slot_" + i));
            }

            rowToWeapon = new Dictionary<VisualElement, Firearms>();
            weaponToAttachments = new Dictionary<VisualElement, VisualElement[]>();
            for (int i = 0; i < weaponRowList.Count; i++)
            {
                VisualElement weaponRow = weaponRowList[i];
                rowToWeapon.Add(weaponRow, null);

                VisualElement weaponSlot = weaponRow.Q<VisualElement>("Weapon");
                weaponSlot.RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopUpWeaponSpecific(weaponSlot)));

                // test for add-on slots on click
                VisualElement[] attachmentSlots = new VisualElement[attachmentPerWeapon];
                for (int j = 0; j < attachmentPerWeapon; j++)
                {
                    VisualElement attachmentSlot = weaponRow.Q<VisualElement>("Attachment_" + j);
                    attachmentSlots[j] = attachmentSlot;
                    attachmentSlot.RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopUpAttachmentInventory(attachmentSlot)));
                }
                weaponToAttachments.Add(weaponSlot, attachmentSlots);
            }

            specificPanel = root.Q<VisualElement>("Specific");
            specificPanel.style.display = DisplayStyle.None;
            specificPanel.RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopOffWeaponSpecific()));

            attachmentsPanel = root.Q<VisualElement>("Attachments");
            attachmentsPanel.style.display = DisplayStyle.None;
            attachmentsPanel.RegisterCallback<MouseDownEvent>(evt => StartCoroutine(PopOffAttachmentInventory()));

            attachmentInventoryRowList = new List<VisualElement>();
            attachmentInventorySlotList = new List<VisualElement>();
            for (int i = 0; i < attachmentsPanel.childCount; i++)
            {
                VisualElement row = attachmentsPanel.Q<VisualElement>("Row_" + i);
                attachmentInventoryRowList.Add(row);
                for (int j = 0; j < row.childCount; j++)
                {
                    VisualElement slot = row.Q<VisualElement>("Slot_" + j);
                    attachmentInventorySlotList.Add(slot);
                }
            }

            // test
            weaponSlots.RegisterCallback<GeometryChangedEvent>(evt => height = weaponSlots.resolvedStyle.height / weaponSlots.childCount);
        }

        private void Start()
        {

        }

        public void ResetView()
        {
            StartCoroutine(PopOffAttachmentInventory());
            StartCoroutine(PopUpWeaponSpecific(selectedWeaponSlot));
        }

        private void SetWeaponToSlot(VisualElement slot, Firearms weapon)
        {
            VisualElement weaponSlot = slot.Q<VisualElement>("Weapon");
            weaponSlot.style.backgroundImage = new StyleBackground(weapon.iconImage);
            weaponSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

            rowToWeapon[slot] = weapon;

            // todo set weapon addons to addon slots
        }

        private IEnumerator PopUpWeaponSpecific(VisualElement weaponSlot)
        {
            Firearms weapon = rowToWeapon[weaponSlot.parent];

            if (weapon == null)
            {
                yield return StartCoroutine(FadeOut(specificPanel));
            }
            else if (selectedWeaponSlot != weaponSlot)
            {
                selectedWeaponSlot = weaponSlot;

                yield return StartCoroutine(FadeOut(specificPanel));

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

        private IEnumerator PopUpAttachmentInventory(VisualElement attachmentSlot)
        {
            Firearms weapon = rowToWeapon[attachmentSlot.parent];

            if (weapon == null)
            {
                yield return null;
            }
            else if (selectedAttachmentSlot != attachmentSlot)
            {
                prevRowPosition.x = attachmentSlot.parent.resolvedStyle.top;
                prevRowPosition.y = attachmentSlot.parent.resolvedStyle.left;

                selectedAttachmentSlot = attachmentSlot;

                StartCoroutine(TranslateTo(attachmentSlot.parent, 0f, 0f));
                StartCoroutine(FadeIn(attachmentsPanel));

                foreach (VisualElement row in weaponRowList)
                {
                    if (row != attachmentSlot.parent)
                        StartCoroutine(FadeOut(row));
                }

                // todo - change specific to selected add-on
            }
        }

        private IEnumerator PopOffAttachmentInventory()
        {
            if (selectedAttachmentSlot != null)
            {
                VisualElement weaponRow = selectedAttachmentSlot.parent;
                StartCoroutine(TranslateTo(weaponRow, prevRowPosition.x, prevRowPosition.y));
                StartCoroutine(FadeOut(attachmentsPanel));

                foreach (VisualElement row in weaponRowList)
                {
                    if (row != weaponRow)
                        StartCoroutine(FadeIn(row));
                }
                selectedAttachmentSlot = null;
            }
            yield return null;
        }

        public void UpdateWeapons(Firearms[] weapons)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    SetWeaponToSlot(weaponRowList[i], weapons[i]);
                }
            }
        }

        public void UpdateAttachmentList(List<Attachment> attachmentList)
        {
            if (attachmentList.Count > 0)
            {
                for (int i = 0; i < attachmentInventorySlotList.Count; i++)
                {
                    Attachment attachment = attachmentList[i];
                    VisualElement slot = attachmentInventorySlotList[i];
                    slot.Q<VisualElement>("AttachmentIcon").style.backgroundImage = new StyleBackground(attachment.iconImage);
                    slot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                }
            }
        }
    }
}
