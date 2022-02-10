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
        private WeaponRowsControl weaponRowsControl;
        private AttachmentInventoryControl attachmentInventoryControl;

        private VisualElement specificPanel;

        private VisualElement selectedAttachmentInventorySlot;
        private VisualElement selectedWeaponSlot;
        private VisualElement selectedEquippedAttachmentSlot;
        private Firearms selectedWeapon;
        private Attachment selectedAttachment;

        private Vector2 previousWeaponRowPosition;


        private static WeaponsPanelControl instance;
        public static WeaponsPanelControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            Initialize();

            specificPanel = root.Q<VisualElement>("ItemSpecific");
            specificPanel.style.display = DisplayStyle.Flex;

        }

        private void Start()
        {
            weaponRowsControl = WeaponRowsControl.Instance;
            attachmentInventoryControl = AttachmentInventoryControl.Instance;

            foreach (var weaponRow in weaponRowsControl.rowList)
            {
                weaponRow.weaponSlot.RegisterCallback<MouseDownEvent>(evt => WeaponSlot_performed(weaponRow.weaponSlot));

                foreach (var attachmentSlot in weaponRow.attachmentSlots)
                {
                    attachmentSlot.RegisterCallback<MouseDownEvent>(evt => EquippedAttachmentSlot_performed(attachmentSlot));
                }
            }

            foreach (var inventorySlot in attachmentInventoryControl.slotList)
            {
                inventorySlot.slot.RegisterCallback<MouseDownEvent>(evt => AttachmentInventorySlot_performed(inventorySlot.slot));
            }
        }



        /*
         *  general
         */

        private void SelectSlot(VisualElement slot)
        {
            Firearms weapon = SelectWeapon(slot);

            Attachment equippedAttachment = SelectEquippedAttachment(slot);

            Attachment inventoryAttachment = SelectInventoryAttachment(slot);

            UpdateSlotsVisual();
        }

        private Firearms SelectWeapon(VisualElement slot)
        {
            Firearms weapon = null;

            if (slot.name == "Weapon")
            {
                weapon = weaponRowsControl.FindWeaponByRow(slot.parent);

                if (selectedWeaponSlot != slot && !attachmentInventoryControl.IsInventoryOpened())
                {
                    selectedWeapon = weapon;
                }

                selectedWeaponSlot = slot;
            }

            return weapon;
        }
        private Attachment SelectEquippedAttachment(VisualElement slot)
        {

            Attachment attachment = null;

            string slotName = slot.name;
            if (slotName.Substring(0, slotName.Length - 1) == "Attachment_")
            {
                selectedWeapon = null;

                attachment = weaponRowsControl.GetEquippedAttachment(slot);

                if (selectedEquippedAttachmentSlot != slot)
                {

                    if (attachment != null)
                    {
                        selectedAttachment = attachment;
                    }

                    selectedEquippedAttachmentSlot = slot;
                }
            }

            return attachment;
        }



        private Attachment SelectInventoryAttachment(VisualElement slot)
        {
            Attachment attachment = null;

            string slotName = slot.name;
            if (slotName.Substring(0, slotName.Length - 1) == "AttachmentInventorySlot_")
            {
                attachment = attachmentInventoryControl.FindAttachmentBySlot(slot);

                if (selectedAttachmentInventorySlot != slot)
                {
                    selectedAttachment = attachment;

                    selectedAttachmentInventorySlot = slot;
                }
            }

            return attachment;
        }



        public void ResetPanel()
        {
            StartCoroutine(PopOffAttachmentInventory());
            StartCoroutine(PopOffSpecific());
        }

        public void UpdateWeapons(Firearms[] weapons)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    weaponRowsControl.SetWeaponToRow(i, weapons[i]);
                }
            }
        }

        public void UpdateSlotsVisual()
        {

            foreach (var row in weaponRowsControl.rowList)
            {
                Firearms weapon = row.weapon;

                if (weapon == null)
                    break;

                VisualElement weaponSlot = row.weaponSlot;

                ApplySelectedVfx(weaponSlot, weapon == selectedWeapon);

                Attachment[] attachments = weapon.Attachments;
                for (int i = 0; i < attachments.Length; i++)
                {
                    VisualElement attachmentSlot = row.attachmentSlots[i];
                    VisualElement attachmentIconSlot = row.attachmentIconSlots[i];
                    if (attachments[i] == null)
                    {
                        attachmentIconSlot.style.backgroundImage = null;
                    }
                    ApplySelectedVfx(attachmentSlot, attachmentSlot == selectedEquippedAttachmentSlot);
                }
            }

            foreach (var inventorySlot in attachmentInventoryControl.slotList)
            {
                VisualElement slot = inventorySlot.slot;
                if (inventorySlot.attachment == null)
                {
                    ApplySelectedVfx(slot, false);
                    slot.style.backgroundColor = Color.clear;
                }
                else
                {
                    ApplySelectedVfx(slot, inventorySlot.attachment == selectedAttachment);

                    if (inventorySlot.attachment.attachedTo != null)
                    {
                        slot.style.backgroundColor = Color.gray;
                    }
                    else
                    {
                        slot.style.backgroundColor = Color.clear;
                    }
                }
            }

        }



        /*
         *  weapon slot
         */

        private void WeaponSlot_performed(VisualElement weaponSlot)
        {
            StartCoroutine(PopUpWeaponSpecific(weaponSlot));

            SelectSlot(weaponSlot);

        }

        private void EquippedAttachmentSlot_performed(VisualElement attachmentSlot)
        {
            StartCoroutine(PopUpAttachmentInventory(attachmentSlot));

            Attachment attachment = weaponRowsControl.GetEquippedAttachment(attachmentSlot);

            if (attachment != null)
            {
                if (selectedEquippedAttachmentSlot == attachmentSlot)
                {
                    Firearms weapon = weaponRowsControl.FindWeaponByRow(attachmentSlot.parent);
                    weapon.RemoveAttachment(attachment);
                    selectedAttachment = null;
                    StartCoroutine(PopOffSpecific());
                }
                else 
                { 
                    StartCoroutine(PopUpAttachmentSpecific(attachment));
                }
            }
            else
            {
                selectedAttachment = null;
            }

            SelectSlot(attachmentSlot);
        }



        /*
         *  attachment inventory
         */

        private void AttachmentInventorySlot_performed(VisualElement attachmentInventorySlot)
        {
            if (selectedAttachmentInventorySlot != attachmentInventorySlot)
            {
                StartCoroutine(PopUpAttachmentSpecific(attachmentInventorySlot));

                SelectSlot(attachmentInventorySlot);

                return;
            }
            else
            {

                Attachment attachment = attachmentInventoryControl.FindAttachmentBySlot(attachmentInventorySlot);
                if (attachment == null)
                    return;

                selectedAttachment = null;

                Firearms weapon = attachment.attachedTo;
                if (weapon != null)
                {
                    weapon.RemoveAttachment(attachment);
                }

                WeaponRow weaponRow = weaponRowsControl.GetWeaponRow(selectedEquippedAttachmentSlot.parent);

                Firearms newWeapon = weaponRow.weapon;

                int index = weaponRowsControl.GetAttachmentSlotIndex(selectedEquippedAttachmentSlot);
                newWeapon.SetAttachment(attachment, index);

                weaponRow.attachmentIconSlots[index].style.backgroundImage = new StyleBackground(attachment.iconImage);
                weaponRow.attachmentIconSlots[index].style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

                selectedAttachmentInventorySlot = null;

                UpdateSlotsVisual();

                StartCoroutine(PopOffAttachmentInventory());
                StartCoroutine(PopOffSpecific());
            }
        }

        public void FlipInventoryPage(int steps)
        {
            attachmentInventoryControl.FlipInventoryPage(steps);

            selectedAttachmentInventorySlot = null;
        }

        private IEnumerator PopUpAttachmentInventory(VisualElement attachmentSlot)
        {
            UpdateSlotsVisual();

            VisualElement attachmentsInventory = attachmentInventoryControl.attachmentsInventory;

            Firearms weapon = weaponRowsControl.FindWeaponByRow(attachmentSlot.parent);

            if (weapon == null)
            {
                yield return null;
            }
            else if (attachmentsInventory.style.display == DisplayStyle.None)
            {
                previousWeaponRowPosition.x = attachmentSlot.parent.resolvedStyle.top;
                previousWeaponRowPosition.y = attachmentSlot.parent.resolvedStyle.left;

                StartCoroutine(TranslateTo(attachmentSlot.parent, 0f, 0f));
                StartCoroutine(FadeIn(attachmentsInventory));

                foreach (WeaponRow weaponRow in weaponRowsControl.rowList)
                {
                    if (weaponRow.row != attachmentSlot.parent)
                        StartCoroutine(FadeOut(weaponRow.row));
                }
            }
        }

        private IEnumerator PopOffAttachmentInventory()
        {
            if (selectedEquippedAttachmentSlot != null)
            {
                VisualElement selectedWeaponRow = selectedEquippedAttachmentSlot.parent;
                StartCoroutine(TranslateTo(selectedWeaponRow, previousWeaponRowPosition.x, previousWeaponRowPosition.y));
                StartCoroutine(FadeOut(attachmentInventoryControl.attachmentsInventory));

                foreach (WeaponRow weaponRow in weaponRowsControl.rowList)
                {
                    if (weaponRow.row != selectedWeaponRow)
                        StartCoroutine(FadeIn(weaponRow.row));
                }

                selectedEquippedAttachmentSlot = null;

                UpdateSlotsVisual();
            }
            yield return null;
        }



        /*
         *  specific 
         */

        private IEnumerator PopUpAttachmentSpecific(VisualElement attachmentInventorySlot)
        {
            Attachment attachment = attachmentInventoryControl.FindAttachmentBySlot(attachmentInventorySlot);

            if (attachment == null)
            {
                yield return StartCoroutine(PopOffSpecific());
            }
            else if (selectedAttachmentInventorySlot != attachmentInventorySlot)
            {
                PopUpSpecific(attachment.BuildDescription());
            }
        }

        private IEnumerator PopUpAttachmentSpecific(Attachment attachment)
        {
            if (attachment == null)
            {
                yield return StartCoroutine(PopOffSpecific());
            }
            else 
            {
                PopUpSpecific(attachment.BuildDescription());
            }
        }

        
        private IEnumerator PopUpWeaponSpecific(VisualElement weaponSlot)
        {
            Firearms weapon = weaponRowsControl.FindWeaponByRow(weaponSlot.parent);

            if (weapon == null)
            {
                yield return StartCoroutine(PopOffSpecific());
            }
            else if (selectedWeaponSlot != weaponSlot && attachmentInventoryControl.attachmentsInventory.style.display == DisplayStyle.None)
            {
                PopUpSpecific(weapon.BuildDescription());
            }
        }

        private void PopUpSpecific(string description)
        {
            specificPanel.Q<Label>("Description").text = description;

            specificPanel.style.opacity = 1f;
            specificPanel.style.display = DisplayStyle.Flex;
        }

        private IEnumerator PopOffSpecific()
        {
            selectedWeaponSlot = null;

            yield return StartCoroutine(FadeOut(specificPanel));
        }

    }
}
