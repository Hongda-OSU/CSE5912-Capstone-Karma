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
        private Dictionary<VisualElement, VisualElement[]> weaponSlotToAttachmentSlots;

        // attachment inventory
        private bool isFadingFinished = true;
        [SerializeField] private List<Attachment> attachmentList;
        private Dictionary<VisualElement, Attachment> inventorySlotToAttachment;
        private List<VisualElement> attachmentInventoryRowList;
        private List<VisualElement> attachmentInventorySlotList;

        // specific

        // others
        private VisualElement selectedElement;
        private VisualElement selectedWeaponSlot;
        private VisualElement selectedAttachmentSlot;
        private VisualElement selectedWeaponRow;
        private VisualElement selectedAttachmentInventorySlot;

        private Vector2 prevRowPosition;
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
            weaponSlotToAttachmentSlots = new Dictionary<VisualElement, VisualElement[]>();
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
                    attachmentSlot.RegisterCallback<MouseDownEvent>(evt => AttachmentSlot_performed(attachmentSlot));
                }

                weaponSlotToAttachmentSlots.Add(weaponSlot, attachmentSlots);
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
            inventorySlotToAttachment = new Dictionary<VisualElement, Attachment>();
            attachmentInventoryRowList = new List<VisualElement>();
            attachmentInventorySlotList = new List<VisualElement>();
            for (int i = 0; i < attachmentsPanel.Q<VisualElement>("Slots").childCount; i++)
            {
                VisualElement row = attachmentsPanel.Q<VisualElement>("Row_" + i);
                attachmentInventoryRowList.Add(row);
                for (int j = 0; j < row.childCount; j++)
                {
                    VisualElement slot = row.Q<VisualElement>("Slot_" + j);
                    attachmentInventorySlotList.Add(slot);
                    slot.RegisterCallback<MouseDownEvent>(evt => AttachmentInventorySlot_performed(slot));
                }
            }



            // test
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

                    inventorySlotToAttachment.Add(slot, attachment);

                    slot.Q<VisualElement>("AttachmentIcon").style.backgroundImage = new StyleBackground(attachment.iconImage);
                    slot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                }
            }
        }

        private void UpdateSelectedElement(VisualElement selected)
        {
            if (selectedElement != null)
                selectedElement.style.backgroundColor = Color.clear;

            selectedElement = selected;
            selectedElement.style.backgroundColor = Color.red;
        }

        /*
         *  weapon slot
         */

        private void WeaponSlot_performed(VisualElement weaponSlot)
        {
            UpdateSelectedElement(weaponSlot);

            StartCoroutine(PopUpWeaponSpecific(weaponSlot));
        }

        private void AttachmentSlot_performed(VisualElement attachmentSlot)
        {

            if (selectedAttachmentSlot != null)
                selectedAttachmentSlot.style.backgroundColor = Color.clear;

            selectedAttachmentSlot = attachmentSlot;
            selectedAttachmentSlot.style.backgroundColor = Color.red;


            StartCoroutine(PopUpAttachmentInventory(attachmentSlot));
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
            UpdateSelectedElement(attachmentInventorySlot);


            if (selectedAttachmentInventorySlot != attachmentInventorySlot)
            {
                isFadingFinished = false;

                UpdateAttachmentInventory();

                StartCoroutine(PopUpAttachmentSpecific(attachmentInventorySlot));

                selectedAttachmentInventorySlot = attachmentInventorySlot;

                return;
            }
            else if (isFadingFinished)
            {

                Attachment attachment = inventorySlotToAttachment[attachmentInventorySlot];

                Firearms weapon = attachment.attachedTo;
                if (weapon != null)
                {
                    weapon.RemoveAttachment(attachment);
                }

                Firearms newWeapon = rowToWeapon[selectedAttachmentSlot.parent];

                VisualElement weaponSlot = selectedWeaponRow.Q<VisualElement>("Weapon");
                int index = Array.IndexOf(weaponSlotToAttachmentSlots[weaponSlot], selectedAttachmentSlot);
                newWeapon.SetAttachment(attachment, index);

                selectedAttachmentSlot.style.backgroundImage = new StyleBackground(attachment.iconImage);
                selectedAttachmentSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

                UpdateAttachmentInventory();

                StartCoroutine(PopOffAttachmentInventory());
                StartCoroutine(PopOffSpecific());
            }

        }

        private void UpdateAttachmentInventory()
        {
            foreach (var item in inventorySlotToAttachment)
            {
                if (item.Value.attachedTo != null)
                {
                    item.Key.style.backgroundColor = Color.gray;
                }
                else
                    item.Key.style.backgroundColor = Color.clear;
            }

            if (selectedElement != null)
                selectedElement.style.backgroundColor = Color.red;

            foreach (var row in weaponRowList)
            {
                if (rowToWeapon[row] == null)
                    break;

                Attachment[] attachments = rowToWeapon[row].Attachments;
                for (int i = 0; i < attachments.Length; i++)
                {
                    if (attachments[i] == null)
                    {
                        VisualElement weaponSlot = row.Q<VisualElement>("Weapon");
                        VisualElement attachmentSlot = weaponSlotToAttachmentSlots[weaponSlot][i];
                        attachmentSlot.style.backgroundImage = null;
                    }
                }
            }
        }
        private IEnumerator PopUpAttachmentInventory(VisualElement attachmentSlot)
        {
            UpdateAttachmentInventory();

            Firearms weapon = rowToWeapon[attachmentSlot.parent];

            if (weapon == null)
            {
                yield return null;
            }
            else if (selectedWeaponRow != attachmentSlot.parent)
            {
                prevRowPosition.x = attachmentSlot.parent.resolvedStyle.top;
                prevRowPosition.y = attachmentSlot.parent.resolvedStyle.left;

                selectedWeaponRow = attachmentSlot.parent;

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

                selectedAttachmentSlot.style.backgroundColor = Color.clear;
                selectedAttachmentSlot = null;
                
                selectedWeaponRow = null;
            }
            yield return null;
        }

        private IEnumerator PopUpAttachmentSpecific(VisualElement attachmentInventorySlot)
        {
            Attachment attachment = inventorySlotToAttachment[attachmentInventorySlot];

            if (attachment == null)
            {
                yield return StartCoroutine(PopOffSpecific());
            }
            else if (selectedAttachmentInventorySlot != attachmentInventorySlot)
            {
                yield return StartCoroutine(PopOffSpecific());

                selectedAttachmentInventorySlot = attachmentInventorySlot;

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

                inventorySlotToAttachment.Add(slot, attachment);

                slot.Q<VisualElement>("AttachmentIcon").style.backgroundImage = new StyleBackground(attachment.iconImage);
                slot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            }
            UpdateAttachmentInventory();
        }

        private void ClearAttachmentInventory()
        {
            inventorySlotToAttachment.Clear();
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
            else if (selectedWeaponSlot != weaponSlot)
            {
                yield return StartCoroutine(PopOffSpecific());

                selectedWeaponSlot = weaponSlot;

                specificPanel.Q<Label>("Description").text = weapon.BuildDescription();

                yield return StartCoroutine(FadeIn(specificPanel));

            }
        }

        private IEnumerator PopOffSpecific()
        {
            selectedWeaponSlot = null;
            selectedAttachmentInventorySlot = null;

            yield return StartCoroutine(FadeOut(specificPanel));

            isFadingFinished = true;
        }





    }
}
