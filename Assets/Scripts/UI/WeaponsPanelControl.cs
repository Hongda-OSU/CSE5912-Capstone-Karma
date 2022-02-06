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
        // panels 
        private VisualElement specificPanel;
        private VisualElement attachmentsPanel;

        // weapon slot
        [SerializeField] private int attachmentPerWeapon = 4;
        private List<VisualElement> weaponRowList;
        private Dictionary<VisualElement, Firearms> rowToWeapon;
        private Dictionary<VisualElement, VisualElement[]> weaponSlotToEquippedAttachmentSlots;

        // attachment inventory
        private bool isFadingFinished = true;
        [SerializeField] private List<Attachment> attachmentList;
        private Dictionary<VisualElement, Attachment> attachmentInventorySlotToAttachment;
        private List<VisualElement> attachmentInventoryRowList;
        private List<VisualElement> attachmentInventorySlotList;

        // specific

        // others
        private VisualElement selectedWeaponSlot;
        private VisualElement selectedEquippedAttachmentSlot;
        private VisualElement selectedWeaponRow;
        private VisualElement selectedAttachmentInventorySlot;

        private Firearms selectedWeapon;
        private Attachment selectedAttachment;

        private Vector2 previousWeaponRowPosition;
        private int currentPage = 0;

        // test

        private void Awake()
        {
            Initialize();


            /*
             * weapons panel
             */

            VisualElement weaponsPanel = root.Q<VisualElement>("WeaponsPanel");

            // find weapon slots
            VisualElement weaponSlots = weaponsPanel.Q<VisualElement>("Slots");
            weaponSlots.style.display = DisplayStyle.Flex;

            weaponRowList = new List<VisualElement>();
            for (int i = 0; i < weaponSlots.childCount; i++)
            {
                weaponRowList.Add(weaponSlots.Q<VisualElement>("Slot_" + i));
            }

            // initialize dictionaries
            rowToWeapon = new Dictionary<VisualElement, Firearms>();
            weaponSlotToEquippedAttachmentSlots = new Dictionary<VisualElement, VisualElement[]>();
            for (int i = 0; i < weaponRowList.Count; i++)
            {
                VisualElement weaponRow = weaponRowList[i];
                rowToWeapon.Add(weaponRow, null);

                // open weapon specific on click
                VisualElement weaponSlot = weaponRow.Q<VisualElement>("Weapon");
                weaponSlot.RegisterCallback<MouseDownEvent>(evt => WeaponSlot_performed(weaponSlot));

                // open attachment inventory on click
                VisualElement[] attachmentSlots = new VisualElement[PlayerInventory.NumOfAttachments];
                for (int j = 0; j < attachmentPerWeapon; j++)
                {
                    VisualElement attachmentSlot = weaponRow.Q<VisualElement>("Attachment_" + j);
                    attachmentSlots[j] = attachmentSlot;
                    attachmentSlot.RegisterCallback<MouseDownEvent>(evt => EquippedAttachmentSlot_performed(attachmentSlot));
                }

                weaponSlotToEquippedAttachmentSlots.Add(weaponSlot, attachmentSlots);
            }


            /*
             *  specific panel
             */

            specificPanel = root.Q<VisualElement>("Specific");
            specificPanel.style.display = DisplayStyle.None;


            /*
             *  attachment inventory panel
             */

            attachmentsPanel = root.Q<VisualElement>("Attachments");
            attachmentsPanel.style.display = DisplayStyle.None;

            // initialize attachment inventory
            attachmentInventorySlotToAttachment = new Dictionary<VisualElement, Attachment>();
            attachmentInventoryRowList = new List<VisualElement>();
            attachmentInventorySlotList = new List<VisualElement>();
            for (int i = 0; i < attachmentsPanel.Q<VisualElement>("AttachmentInventorySlots").childCount; i++)
            {
                VisualElement row = attachmentsPanel.Q<VisualElement>("AttachmentInventoryRow_" + i);
                attachmentInventoryRowList.Add(row);
                for (int j = 0; j < row.childCount; j++)
                {
                    VisualElement slot = row.Q<VisualElement>("AttachmentInventorySlot_" + j);
                    attachmentInventorySlotList.Add(slot);
                    slot.RegisterCallback<MouseDownEvent>(evt => AttachmentInventorySlot_performed(slot));
                }
            }



            // test
        }

        private Attachment GetEquippedAttachment(VisualElement slot)
        {
            Firearms weapon = rowToWeapon[slot.parent];
            int index = GetAttachmentSlotIndex(slot);

            return weapon.Attachments[index];
        }
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

            if (weapon != null)
            {
                Debug.Log(weapon.name);
            }
            else if (equippedAttachment != null)
            {
                Debug.Log(equippedAttachment.name);
            }
            else if (inventoryAttachment != null)
            {
                Debug.Log(inventoryAttachment.name);
            }
            UpdateSlotsVisual();
        }

        private Firearms SelectWeapon(VisualElement slot)
        {
            Firearms weapon = null;

            if (slot.name == "Weapon")
            {
                weapon = rowToWeapon[slot.parent];

                if (selectedWeaponSlot != slot && attachmentsPanel.style.display == DisplayStyle.None)
                {
                    selectedWeapon = weapon;
                }

                selectedWeaponSlot = slot;
                selectedWeaponRow = slot.parent;
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

                attachment = GetEquippedAttachment(slot);

                if (selectedEquippedAttachmentSlot != slot)
                {

                    if (attachment != null)
                    {
                        selectedAttachment = attachment;
                    }

                    selectedEquippedAttachmentSlot = slot;
                }
                selectedWeaponRow = slot.parent;
            }

            return attachment;
        }

        private Attachment SelectInventoryAttachment(VisualElement slot)
        {
            Attachment attachment = null;

            string slotName = slot.name;
            if (slotName.Substring(0, slotName.Length - 1) == "AttachmentInventorySlot_")
            {
                attachment = attachmentInventorySlotToAttachment[slot];

                if (selectedAttachmentInventorySlot != slot)
                {
                    selectedAttachment = attachment;

                    selectedAttachmentInventorySlot = slot;
                }
            }

            return attachment;
        }



        private int GetAttachmentSlotIndex(VisualElement slot)
        {
            VisualElement row = slot.parent;
            VisualElement weaponSlot = row.Q<VisualElement>("Weapon");
            VisualElement[] attachmentSlots = weaponSlotToEquippedAttachmentSlots[weaponSlot];

            return Array.IndexOf(attachmentSlots, slot);
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
                    SetWeaponToSlot(weaponRowList[i], weapons[i]);
                }
            }
        }

        public void UpdateAttachmentList(List<Attachment> attachmentList)
        {
            this.attachmentList = attachmentList;

            ClearAttachmentInventory();

            if (attachmentList.Count > 0)
            {
                for (int i = 0; i < attachmentList.Count; i++)
                {
                    if (i >= attachmentInventorySlotList.Count)
                        return;

                    Attachment attachment = attachmentList[i];
                    VisualElement slot = attachmentInventorySlotList[i];

                    attachmentInventorySlotToAttachment.Add(slot, attachment);

                    slot.Q<VisualElement>("AttachmentIcon").style.backgroundImage = new StyleBackground(attachment.iconImage);
                    slot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
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

            Attachment attachment = GetEquippedAttachment(attachmentSlot);
            if (attachment != null)
            {
                StartCoroutine(PopUpAttachmentSpecific(attachment));
            }

            SelectSlot(attachmentSlot);
        }

        private void SetWeaponToSlot(VisualElement slot, Firearms weapon)
        {
            VisualElement weaponSlot = slot.Q<VisualElement>("Weapon");
            weaponSlot.style.backgroundImage = new StyleBackground(weapon.iconImage);
            weaponSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

            rowToWeapon[slot] = weapon;

            // todo - set weapon addons to addon slots
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

                Attachment attachment = attachmentInventorySlotToAttachment[attachmentInventorySlot];

                selectedAttachment = null;

                Firearms weapon = attachment.attachedTo;
                if (weapon != null)
                {
                    weapon.RemoveAttachment(attachment);
                }

                Firearms newWeapon = rowToWeapon[selectedEquippedAttachmentSlot.parent];

                VisualElement weaponSlot = selectedWeaponRow.Q<VisualElement>("Weapon");
                int index = GetAttachmentSlotIndex(selectedEquippedAttachmentSlot);
                newWeapon.SetAttachment(attachment, index);

                selectedEquippedAttachmentSlot.style.backgroundImage = new StyleBackground(attachment.iconImage);
                selectedEquippedAttachmentSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

                UpdateSlotsVisual();

                StartCoroutine(PopOffAttachmentInventory());
                StartCoroutine(PopOffSpecific());
            }

        }

        private void UpdateSlotsVisual()
        {
            foreach (var item in attachmentInventorySlotToAttachment)
            {
                if (item.Value == selectedAttachment)
                    item.Key.style.backgroundColor = Color.red;
                else if (item.Value.attachedTo != null)
                {
                    item.Key.style.backgroundColor = Color.gray;
                }
                else
                    item.Key.style.backgroundColor = Color.clear;
            }

            foreach (var row in weaponRowList)
            {
                if (rowToWeapon[row] == null)
                    break;

                Firearms weapon = rowToWeapon[row];
                VisualElement weaponSlot = row.Q<VisualElement>("Weapon");
                if (weapon == selectedWeapon)
                    ApplySelectedEffect(weaponSlot, true);
                else
                    ApplySelectedEffect(weaponSlot, false);

                Attachment[] attachments = weapon.Attachments;
                for (int i = 0; i < attachments.Length; i++)
                {
                    VisualElement attachmentSlot = weaponSlotToEquippedAttachmentSlots[weaponSlot][i];
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
        private IEnumerator PopUpAttachmentInventory(VisualElement attachmentSlot)
        {
            UpdateSlotsVisual();

            Firearms weapon = rowToWeapon[attachmentSlot.parent];

            if (weapon == null)
            {
                yield return null;
            }
            else if (attachmentsPanel.style.display == DisplayStyle.None)
            {
                previousWeaponRowPosition.x = attachmentSlot.parent.resolvedStyle.top;
                previousWeaponRowPosition.y = attachmentSlot.parent.resolvedStyle.left;

                StartCoroutine(TranslateTo(attachmentSlot.parent, 0f, 0f));
                StartCoroutine(FadeIn(attachmentsPanel));

                foreach (VisualElement row in weaponRowList)
                {
                    if (row != attachmentSlot.parent)
                        StartCoroutine(FadeOut(row));
                }
            }
        }

        private IEnumerator PopOffAttachmentInventory()
        {
            if (selectedEquippedAttachmentSlot != null)
            {
                VisualElement weaponRow = selectedEquippedAttachmentSlot.parent;
                StartCoroutine(TranslateTo(weaponRow, previousWeaponRowPosition.x, previousWeaponRowPosition.y));
                StartCoroutine(FadeOut(attachmentsPanel));

                foreach (VisualElement row in weaponRowList)
                {
                    if (row != weaponRow)
                        StartCoroutine(FadeIn(row));
                }

                selectedEquippedAttachmentSlot.style.backgroundColor = Color.clear;
                selectedEquippedAttachmentSlot = null;
                
                selectedWeaponRow = null;
            }
            yield return null;
        }

        private IEnumerator PopUpAttachmentSpecific(VisualElement attachmentInventorySlot)
        {
            Attachment attachment = attachmentInventorySlotToAttachment[attachmentInventorySlot];

            if (attachment == null)
            {
                yield return StartCoroutine(PopOffSpecific());
            }
            else if (selectedAttachmentInventorySlot != attachmentInventorySlot)
            {
                yield return StartCoroutine(PopOffSpecific());

                specificPanel.Q<Label>("Description").text = attachment.BuildDescription();

                yield return StartCoroutine(FadeIn(specificPanel));
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

                specificPanel.Q<Label>("Description").text = attachment.BuildDescription();

                yield return StartCoroutine(FadeIn(specificPanel));
            }
        }


        public void FlipInventoryPage(int steps)
        {
            if (attachmentsPanel.style.display != DisplayStyle.Flex)
                return;

            int numPerPage = attachmentInventorySlotList.Count;
            int numOfAttachments = attachmentList.Count;
            int numOfPages = numOfAttachments / numPerPage + 1;

            if (numOfAttachments % numPerPage == 0)
                return;

            currentPage = Mathf.Clamp(currentPage + steps, 0, numOfPages - 1);

            int numOnPage = Mathf.Clamp(numOfAttachments - numPerPage * currentPage, 0, numPerPage);

            ClearAttachmentInventory();

            for (int i = 0; i < numOnPage; i++)
            {
                int index = i + numOnPage * currentPage;
                Attachment attachment = attachmentList[index];
                VisualElement slot = attachmentInventorySlotList[i];

                attachmentInventorySlotToAttachment.Add(slot, attachment);

                slot.Q<VisualElement>("AttachmentIcon").style.backgroundImage = new StyleBackground(attachment.iconImage);
                slot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            }
            UpdateSlotsVisual();
        }

        private void ClearAttachmentInventory()
        {
            attachmentInventorySlotToAttachment.Clear();
            foreach (VisualElement slot in attachmentInventorySlotList)
            {
                slot.Q<VisualElement>("AttachmentIcon").style.backgroundImage = null;
            }
        }



        /*
         *  specific panel
         */

        private IEnumerator PopUpWeaponSpecific(VisualElement weaponSlot)
        {
            Firearms weapon = rowToWeapon[weaponSlot.parent];

            if (weapon == null)
            {
                yield return StartCoroutine(PopOffSpecific());
            }
            else if (selectedWeaponSlot != weaponSlot && attachmentsPanel.style.display == DisplayStyle.None)
            {
                yield return StartCoroutine(PopOffSpecific());

                specificPanel.Q<Label>("Description").text = weapon.BuildDescription();

                yield return StartCoroutine(FadeIn(specificPanel));

            }
        }

        private IEnumerator PopOffSpecific()
        {
            yield return StartCoroutine(FadeOut(specificPanel));

            isFadingFinished = true;
        }





    }
}
