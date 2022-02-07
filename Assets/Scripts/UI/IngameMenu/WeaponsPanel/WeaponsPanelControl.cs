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
        public WeaponRowsControl weaponRowsControl;
        public AttachmentInventoryControl attachmentInventoryControl;

        private VisualElement specificPanel;

        private bool isFadingFinished = true;

        private VisualElement selectedAttachmentInventorySlot;
        private VisualElement selectedWeaponSlot;
        private VisualElement selectedEquippedAttachmentSlot;
        private Firearms selectedWeapon;
        private Attachment selectedAttachment;

        private Vector2 previousWeaponRowPosition;


        private void Awake()
        {
            Initialize();

            specificPanel = root.Q<VisualElement>("Specific");
            specificPanel.style.display = DisplayStyle.None;

        }

        private void Start()
        {
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

        private void ApplySelectedEffect(VisualElement slot, bool isSelected)
        {
            if (slot != null)
            {
                if (isSelected)
                {
                    slot.style.backgroundColor = Color.red;
                }
                else
                {
                    slot.style.backgroundColor = Color.clear;
                }
            }
        }

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

            foreach (var inventorySlot in attachmentInventoryControl.slotList)
            {
                if (inventorySlot.attachment == selectedAttachment)
                    inventorySlot.slot.style.backgroundColor = Color.red;
                else if (inventorySlot.attachment.attachedTo != null)
                {
                    inventorySlot.slot.style.backgroundColor = Color.gray;
                }
                else
                    inventorySlot.slot.style.backgroundColor = Color.clear;
            }

            foreach (var row in weaponRowsControl.rowList)
            {
                Firearms weapon = row.weapon;

                if (weapon == null)
                    break;

                VisualElement weaponSlot = row.weaponSlot;
                if (weapon == selectedWeapon)
                    ApplySelectedEffect(weaponSlot, true);
                else
                    ApplySelectedEffect(weaponSlot, false);

                Attachment[] attachments = weapon.Attachments;
                for (int i = 0; i < attachments.Length; i++)
                {
                    VisualElement attachmentSlot = row.attachmentSlots[i];
                    if (attachments[i] == null)
                    {
                        attachmentSlot.style.backgroundImage = null;
                    }
                    if (attachmentSlot == selectedEquippedAttachmentSlot)
                    {
                        attachmentSlot.style.backgroundColor = Color.red;
                    }
                    else
                    {
                        attachmentSlot.style.backgroundColor = Color.clear;
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
                StartCoroutine(PopUpAttachmentSpecific(attachment));
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

                isFadingFinished = false;

                UpdateSlotsVisual();

                StartCoroutine(PopUpAttachmentSpecific(attachmentInventorySlot));

                SelectSlot(attachmentInventorySlot);

                return;
            }
            else if (isFadingFinished)
            {

                Attachment attachment = attachmentInventoryControl.FindAttachmentBySlot(attachmentInventorySlot);

                selectedAttachment = null;

                Firearms weapon = attachment.attachedTo;
                if (weapon != null)
                {
                    weapon.RemoveAttachment(attachment);
                }

                Firearms newWeapon = weaponRowsControl.FindWeaponByRow(selectedEquippedAttachmentSlot.parent);

                int index = weaponRowsControl.GetAttachmentSlotIndex(selectedEquippedAttachmentSlot);
                newWeapon.SetAttachment(attachment, index);

                selectedEquippedAttachmentSlot.style.backgroundImage = new StyleBackground(attachment.iconImage);
                selectedEquippedAttachmentSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

                selectedAttachmentInventorySlot = null;

                UpdateSlotsVisual();

                StartCoroutine(PopOffAttachmentInventory());
                StartCoroutine(PopOffSpecific());
            }

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
                yield return StartCoroutine(PopOffSpecific());

                yield return StartCoroutine(PopUpSpecific(attachment.BuildDescription()));
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
                yield return StartCoroutine(PopOffSpecific());

                yield return StartCoroutine(PopUpSpecific(attachment.BuildDescription()));
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
                yield return StartCoroutine(PopOffSpecific());

                yield return StartCoroutine(PopUpSpecific(weapon.BuildDescription()));
            }
        }

        private IEnumerator PopUpSpecific(string description)
        {
            specificPanel.Q<Label>("Description").text = description;

            yield return StartCoroutine(FadeIn(specificPanel));
        }

        private IEnumerator PopOffSpecific()
        {
            selectedWeaponSlot = null;

            yield return StartCoroutine(FadeOut(specificPanel));

            isFadingFinished = true;
        }

    }
}
