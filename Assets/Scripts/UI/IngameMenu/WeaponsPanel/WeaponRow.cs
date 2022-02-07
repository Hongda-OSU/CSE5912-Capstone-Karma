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
        public Firearms weapon;

        public VisualElement[] attachmentSlots;
        public Attachment[] attachments;

        public WeaponRow(VisualElement weaponRow)
        {
            if (weaponRow.name.Substring(0, weaponRow.name.Length - 1) != "WeaponRow_")
            {
                Debug.Log("Error: The name of VisualElement is: " + weaponRow.name);
                return;
            }

            row = weaponRow;

            weaponSlot = weaponRow.Q<VisualElement>("Weapon");
            weapon = null;

            int num = PlayerInventory.NumOfAttachmentsPerWeapon;
            attachmentSlots = new VisualElement[num];
            attachments = new Attachment[num];
            for (int i = 0; i < num; i++)
            {
                attachmentSlots[i] = weaponRow.Q<VisualElement>("Attachment_" + i);
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
