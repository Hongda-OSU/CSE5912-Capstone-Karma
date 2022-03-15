using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class WeaponRow
    {
        public VisualElement row;

        public VisualElement weaponSlot;
        public VisualElement weaponIconSlot;
        public Firearms weapon;

        public VisualElement[] attachmentSlots;
        public VisualElement[] attachmentIconSlots;
        public AttachmentItem[] attachments;

        public WeaponRow(VisualElement weaponRow)
        {
            if (weaponRow.name.Substring(0, weaponRow.name.Length - 1) != "WeaponRow_")
            {
                Debug.Log("Error: The name of VisualElement is: " + weaponRow.name);
                return;
            }

            row = weaponRow;

            weaponSlot = weaponRow.Q<VisualElement>("Weapon");
            weaponSlot.style.unityBackgroundImageTintColor = Color.clear;

            weapon = null;

            weaponIconSlot = weaponRow.Q<VisualElement>("WeaponIcon");

            //int num = PlayerInventory.Instance.MaxNumOfAttachmentsPerWeapon;
            int num = 4; // hard coded

            attachmentSlots = new VisualElement[num];
            attachmentIconSlots = new VisualElement[num];
            attachments = new AttachmentItem[num];

            for (int i = 0; i < num; i++)
            {
                attachmentSlots[i] = weaponRow.Q<VisualElement>("Attachment_" + i);
                attachmentSlots[i].style.unityBackgroundImageTintColor = Color.clear;
                attachmentSlots[i].style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

                attachmentIconSlots[i] = attachmentSlots[i].Q<VisualElement>("AttachmentIcon");
            }
        }

        public bool ContainsAttachmentSlot(VisualElement slot)
        {
            foreach (VisualElement attachmentSlot in attachmentSlots)
            {
                if (attachmentSlot == slot)
                    return true;
            }
            return false;
        }

    }
}
