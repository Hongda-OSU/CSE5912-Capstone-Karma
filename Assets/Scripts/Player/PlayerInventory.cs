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

        [SerializeField] private Firearms[] playerWeapons; 

        private List<Attachment> attachmentList;


        private static PlayerInventory instance;
        public static PlayerInventory Instance { get { return instance; } }

        // test
        [SerializeField] Sprite[] attachmentIcons;
        bool yes = false;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            attachmentList = new List<Attachment>();
        }
        private void Start()
        {
            weaponsPanelControl = WeaponsPanelControl.Instance;
            attachmentInventoryControl = AttachmentInventoryControl.Instance;
        }

        private void Update()
        {

            if (!yes)
            {
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
                yes = true;
            }
        }


        public void AddWeapon(Firearms weapon)
        {
            for (int i = 0; i < playerWeapons.Length; i++)
            {
                if (playerWeapons[i] == null)
                {
                    playerWeapons[i] = weapon;
                    break;
                }
            }
            UpdateAll();
        }

        private void UpdateAll()
        {
            WeaponsPanelControl.Instance.UpdateWeapons(playerWeapons);
            AttachmentInventoryControl.Instance.UpdateAttachmentInventory(attachmentList);
            WeaponInformationControl.Instance.UpdateWeapon();
        }

        public List<Firearms> GetPlayerWeaponList()
        {
            return new List<Firearms>(playerWeapons);
        }
    }
}
