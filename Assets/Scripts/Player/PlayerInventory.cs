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
        public static int NumOfAttachmentsPerWeapon { get { return numOfAttachmentsPerWeapon; } }

        private WeaponsPanelControl weaponsPanelControl;
        private AttachmentInventoryControl attachmentInventoryControl;

        [SerializeField] private Firearms[] weapons; 

        private List<Attachment> attachmentList;


        private static PlayerInventory instance;
        public static PlayerInventory Instance { get { return instance; } }

        // test
        [SerializeField] Sprite[] attachmentIcons;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;
        }
        private void Start()
        {
            weaponsPanelControl = WeaponsPanelControl.Instance;
            attachmentInventoryControl = AttachmentInventoryControl.Instance;

            attachmentList = new List<Attachment>();

            // test
            int num = 100;
            for (int i = 0; i < num; i++)
            {
                var attachment = new GameObject();
                attachment.AddComponent<Attachment>();
                var at = attachment.GetComponent<Attachment>();

                at.attachmentName = i.ToString();
                at.iconImage = attachmentIcons[UnityEngine.Random.Range(0, attachmentIcons.Length)];

                attachmentList.Add(at);
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
            attachmentInventoryControl.UpdateAttachmentInventory(attachmentList);
        }
    }
}
