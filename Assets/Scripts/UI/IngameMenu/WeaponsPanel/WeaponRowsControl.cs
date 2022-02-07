using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class WeaponRowsControl : UI
    {
        // weapon slot

        public List<WeaponRow> weaponRowList;

        private void Awake()
        {
            Initialize();

            // find weapon slots
            VisualElement weaponRows = uiDocument.rootVisualElement.Q<VisualElement>("WeaponRows");

            weaponRows.style.display = DisplayStyle.Flex;

            weaponRowList = new List<WeaponRow>();
            for (int i = 0; i < weaponRows.childCount; i++)
            {
                weaponRowList.Add(new WeaponRow(weaponRows.Q<VisualElement>("WeaponRow_" + i)));
            }

        }

        
        public Attachment GetEquippedAttachment(VisualElement slot)
        {
            Firearms weapon = null;

            foreach (WeaponRow weaponRow in weaponRowList)
                if (weaponRow.ContainsAttachmentSlot(slot))
                        weapon = weaponRow.weapon;

            int index = GetAttachmentSlotIndex(slot);

            return weapon.Attachments[index];
        }
        public int GetAttachmentSlotIndex(VisualElement slot)
        {
            int index = -1;

            foreach (WeaponRow weaponRow in weaponRowList)
                if (weaponRow.ContainsAttachmentSlot(slot))
                    index = Array.IndexOf(weaponRow.attachmentSlots, slot);

            return index;
        }

        public Firearms FindWeaponByRow(VisualElement row)
        {
            Firearms weapon = null;

            foreach (WeaponRow weaponRow in weaponRowList)
            {
                if (weaponRow.row == row)
                {
                    weapon = weaponRow.weapon;
                }
            }

            return weapon;
        }

        public void SetWeaponToRow(int index, Firearms weapon)
        {
            WeaponRow weaponRow = weaponRowList[index];

            weaponRow.weapon = weapon;

            weaponRow.weaponSlot.style.backgroundImage = new StyleBackground(weapon.iconImage);
            weaponRow.weaponSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

        }


    }

    
}
