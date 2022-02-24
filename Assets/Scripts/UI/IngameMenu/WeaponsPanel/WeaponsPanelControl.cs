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
        [SerializeField] private Color commonColor = Color.white;
        [SerializeField] private Color rareColor = Color.blue;
        [SerializeField] private Color epicColor = Color.magenta;
        [SerializeField] private Color legendaryColor = Color.red;
        [SerializeField] private Color divineColor = Color.cyan;
        private Dictionary<Firearms.WeaponRarity, Color> weaponRarityToColor;
        private Dictionary<Attachment.AttachmentRarity, Color> attachmentRarityToColor;

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

            weaponRarityToColor = new Dictionary<Firearms.WeaponRarity, Color>();
            weaponRarityToColor.Add(Firearms.WeaponRarity.Common, commonColor);
            weaponRarityToColor.Add(Firearms.WeaponRarity.Rare, rareColor);
            weaponRarityToColor.Add(Firearms.WeaponRarity.Epic, epicColor);
            weaponRarityToColor.Add(Firearms.WeaponRarity.Legendary, legendaryColor);
            weaponRarityToColor.Add(Firearms.WeaponRarity.Divine, divineColor);

            attachmentRarityToColor = new Dictionary<Attachment.AttachmentRarity, Color>();
            attachmentRarityToColor.Add(Attachment.AttachmentRarity.Common, commonColor);
            attachmentRarityToColor.Add(Attachment.AttachmentRarity.Rare, rareColor);
            attachmentRarityToColor.Add(Attachment.AttachmentRarity.Epic, epicColor);
            attachmentRarityToColor.Add(Attachment.AttachmentRarity.Legendary, legendaryColor);
            attachmentRarityToColor.Add(Attachment.AttachmentRarity.Divine, divineColor);

            specificPanel = root.Q<VisualElement>("ItemSpecific");
            specificPanel.style.display = DisplayStyle.None;

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

                if (selectedWeaponSlot != slot)
                {
                    selectedWeapon = weapon;
                }

                selectedWeaponSlot = slot;
                selectedEquippedAttachmentSlot = null;
                selectedAttachmentInventorySlot = null;
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
                    selectedWeaponSlot = null;
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
                selectedWeaponSlot = null;
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

                    if (inventorySlot.attachment.AttachedTo != null)
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
            if (attachmentInventoryControl.attachmentsInventory.style.display == DisplayStyle.Flex)
            {
                StartCoroutine(PopOffAttachmentInventory());
            }
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

                Firearms weapon = attachment.AttachedTo;
                if (weapon != null)
                {
                    weapon.RemoveAttachment(attachment);
                }

                WeaponRow weaponRow = weaponRowsControl.GetWeaponRow(selectedEquippedAttachmentSlot.parent);

                Firearms newWeapon = weaponRow.weapon;

                int index = weaponRowsControl.GetAttachmentSlotIndex(selectedEquippedAttachmentSlot);
                newWeapon.SetAttachment(attachment, index);

                weaponRow.attachmentIconSlots[index].style.backgroundImage = new StyleBackground(attachment.IconImage);
                weaponRow.attachmentIconSlots[index].style.unityBackgroundImageTintColor = Instance.AttachmentRarityToColor[attachment.Rarity];
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
                selectedAttachment = null;

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
                PopUpSpecific(attachment);
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
                PopUpSpecific(attachment);
            }
        }

        
        private IEnumerator PopUpWeaponSpecific(VisualElement weaponSlot)
        {
            Firearms weapon = weaponRowsControl.FindWeaponByRow(weaponSlot.parent);

            if (weapon == null)
            {
                yield return StartCoroutine(PopOffSpecific());
            }
            else if (selectedWeaponSlot != weaponSlot)
            {
                PopUpSpecific(weapon);
            }
        }

        private void PopUpSpecific(Firearms weapon)
        {
            specificPanel.Q<VisualElement>("WeaponSpecific").style.display = DisplayStyle.Flex;
            specificPanel.Q<VisualElement>("AttachmentSpecific").style.display = DisplayStyle.None;


            Color color = weaponRarityToColor[weapon.Rarity];

            specificPanel.style.borderBottomColor = color;
            specificPanel.style.borderLeftColor = color;
            specificPanel.style.borderRightColor = color;


            VisualElement title = specificPanel.Q<VisualElement>("WeaponSpecific").Q<VisualElement>("Title");
            VisualElement specific = specificPanel.Q<VisualElement>("WeaponSpecific").Q<VisualElement>("Specific");
            VisualElement bonus = specificPanel.Q<VisualElement>("WeaponSpecific").Q<VisualElement>("Bonus");


            title.Q<Label>("Name").text = weapon.WeaponName;
            title.Q<Label>("Name").style.color = color;

            title.Q<Label>("Type").text = weapon.Type.ToString();


            //specific.Q<VisualElement>("Rarity").Q<Label>("Label").style.color = rarityToColor[weapon.Rarity];
            specific.Q<VisualElement>("Rarity").Q<Label>("Data").text = weapon.Rarity.ToString();
            specific.Q<VisualElement>("Rarity").Q<Label>("Data").style.color = color;

            //specific.Q<VisualElement>("Damage").Q<Label>("Label").style.color = rarityToColor[weapon.Rarity];
            specific.Q<VisualElement>("Damage").Q<Label>("Data").text = weapon.Damage.ToString();

            //specific.Q<VisualElement>("Element").Q<Label>("Label").style.color = rarityToColor[weapon.Rarity];
            specific.Q<VisualElement>("Element").Q<Label>("Data").text = weapon.Element.ToString();
            specific.Q<VisualElement>("Element").Q<Label>("Data").style.color = Element.Instance.TypeToColor[weapon.Element];

            //specific.Q<VisualElement>("Ammo").Q<Label>("Label").style.color = rarityToColor[weapon.Rarity];
            specific.Q<VisualElement>("Ammo").Q<Label>("Data").text = weapon.AmmoInMag.ToString();


            var list = weapon.Bonus.GetBonusDescriptionList();
            int num = 0;
            foreach (var child in bonus.Children())
            {
                if (num < list.Count)
                {
                    child.Q<Label>("Data").text = list[num];
                    child.style.display = DisplayStyle.Flex;
                }
                else
                {
                    child.style.display = DisplayStyle.None;
                }
                num++;
            }

            specificPanel.style.opacity = 1f;
            specificPanel.style.display = DisplayStyle.Flex;
        }

        private void PopUpSpecific(Attachment attachment)
        {
            specificPanel.Q<VisualElement>("WeaponSpecific").style.display = DisplayStyle.None;
            specificPanel.Q<VisualElement>("AttachmentSpecific").style.display = DisplayStyle.Flex;


            Color color = attachmentRarityToColor[attachment.Rarity];

            specificPanel.style.borderBottomColor = color;
            specificPanel.style.borderLeftColor = color;
            specificPanel.style.borderRightColor = color;


            VisualElement title = specificPanel.Q<VisualElement>("AttachmentSpecific").Q<VisualElement>("Title");
            VisualElement specific = specificPanel.Q<VisualElement>("AttachmentSpecific").Q<VisualElement>("Specific");
            VisualElement bonus = specificPanel.Q<VisualElement>("AttachmentSpecific").Q<VisualElement>("Bonus");


            title.Q<Label>("Name").text = attachment.AttachmentName;
            title.Q<Label>("Name").style.color = color;

            title.Q<Label>("Type").text = attachment.Type.ToString();

            specific.Q<VisualElement>("Rarity").Q<Label>("Data").text = attachment.Rarity.ToString();
            specific.Q<VisualElement>("Rarity").Q<Label>("Data").style.color = color;

            var list = attachment.Bonus.GetBonusDescriptionList();
            int num = 0;
            foreach (var child in bonus.Children())
            {
                if (num < list.Count)
                {
                    child.Q<Label>("Data").text = list[num];
                    child.style.display = DisplayStyle.Flex;
                }
                else
                {
                    child.style.display = DisplayStyle.None;
                }
                num++;
            }

            specificPanel.style.opacity = 1f;
            specificPanel.style.display = DisplayStyle.Flex;
        }

        private IEnumerator PopOffSpecific()
        {
            selectedWeaponSlot = null;

            yield return StartCoroutine(FadeOut(specificPanel));

            specificPanel.style.opacity = 0f;
        }


        public Dictionary<Firearms.WeaponRarity, Color> WeaponRarityToColor { get { return weaponRarityToColor; } }
        public Dictionary<Attachment.AttachmentRarity, Color> AttachmentRarityToColor { get { return attachmentRarityToColor; } }
    }
}
