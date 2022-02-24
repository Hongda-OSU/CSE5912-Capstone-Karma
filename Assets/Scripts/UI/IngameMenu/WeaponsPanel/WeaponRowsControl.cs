using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class WeaponRowsControl : UI
    {
        public List<WeaponRow> rowList;

        private VisualElement weaponRows;

        private static WeaponRowsControl instance;
        public static WeaponRowsControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            Initialize();

            // find weapon slots
            weaponRows = root.Q<VisualElement>("WeaponRows");

            weaponRows.style.display = DisplayStyle.Flex;

            rowList = new List<WeaponRow>();

            for (int i = 0; i < weaponRows.childCount; i++)
            {
                rowList.Add(new WeaponRow(weaponRows.Q<VisualElement>("WeaponRow_" + i)));
            }
        }

        private void Start()
        {
        }

        public WeaponRow GetWeaponRow(VisualElement row)
        {
            foreach (var weaponRow in rowList)
            {
                if (weaponRow.row == row)
                    return weaponRow;
            }
            return null;
        }

        public Attachment GetEquippedAttachment(VisualElement slot)
        {
            Firearms weapon = null;

            foreach (WeaponRow weaponRow in rowList)
                if (weaponRow.ContainsAttachmentSlot(slot))
                        weapon = weaponRow.weapon;

            if (weapon == null)
                return null;

            int index = GetAttachmentSlotIndex(slot);

            return weapon.Attachments[index];
        }
        public int GetAttachmentSlotIndex(VisualElement slot)
        {
            int index = -1;

            foreach (WeaponRow weaponRow in rowList)
                if (weaponRow.ContainsAttachmentSlot(slot))
                    index = Array.IndexOf(weaponRow.attachmentSlots, slot);

            return index;
        }

        public Firearms FindWeaponByRow(VisualElement row)
        {
            Firearms weapon = null;

            foreach (WeaponRow weaponRow in rowList)
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
            WeaponRow weaponRow = rowList[index];
            VisualElement iconSlot = weaponRow.weaponIconSlot;

            weaponRow.weapon = weapon;

            iconSlot.style.backgroundImage = new StyleBackground(weapon.iconImage);
            iconSlot.style.unityBackgroundImageTintColor = WeaponsPanelControl.Instance.WeaponRarityToColor[weapon.Rarity];
            iconSlot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        public void SetAttachment()
        {

        }

    }

    
}
