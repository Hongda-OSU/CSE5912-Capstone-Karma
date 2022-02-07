using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private static int numOfAttachmentsPerWeapon = 4;
        public static int NumOfAttachmentsPerWeapon
        {
            get { return numOfAttachmentsPerWeapon; }
        }

        [SerializeField] private WeaponsPanelControl weaponsPanelControl;

        [SerializeField] private Firearms[] weapons; 

        [SerializeField] private List<Attachment> attachmentList;

        // test
        [SerializeField] Sprite[] attachmentIcons;

        private void Awake()
        {
        }
        private void Start()
        {
            attachmentList = new List<Attachment>();

            // test
            int num = 100;
            for (int i = 0; i < num; i++)
            {
                var attachment = new Attachment();
                attachment.name = i.ToString();
                attachment.iconImage = attachmentIcons[UnityEngine.Random.Range(0, attachmentIcons.Length)];

                attachmentList.Add(attachment);
            }


            UpdateAll();

        }

        public void AddWeapon(Firearms weapon)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] == null)
                {
                    weapons[i] = weapon;
                    break;
                }
            }
            UpdateAll();
        }

        private void UpdateAll()
        {
            weaponsPanelControl.UpdateWeapons(weapons);
            weaponsPanelControl.UpdateAttachmentList(attachmentList);
        }
    }
}
