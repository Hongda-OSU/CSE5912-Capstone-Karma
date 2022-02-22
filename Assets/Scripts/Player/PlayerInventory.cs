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
        [SerializeField] private int maxNumOfWeapons = 5;
        [SerializeField] private int maxNumOfAttachmentsPerWeapon = 4;

        [SerializeField] private Firearms defaultWeapon;

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

            playerWeapons = new Firearms[maxNumOfWeapons];
            attachmentList = new List<Attachment>();
        }
        private void Start()
        {
            weaponsPanelControl = WeaponsPanelControl.Instance;
            attachmentInventoryControl = AttachmentInventoryControl.Instance;
        }

        private void Update()
        {
            if (!Contains(defaultWeapon))
                AddWeapon(defaultWeapon);

            if (!yes)
            {
                // test
                GameObject testAttachments = new GameObject();
                int num = 100;
                for (int i = 0; i < num; i++)
                {
                    var attachment = new GameObject();
                    attachment.AddComponent<Attachment>();
                    attachment.transform.SetParent(testAttachments.transform);

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

        public bool Contains(Firearms target)
        {
            foreach (var weapon in playerWeapons)
            {
                if (weapon == target)
                    return true;
            }
            return false;
        }

        private void UpdateAll()
        {
            WeaponsPanelControl.Instance.UpdateWeapons(playerWeapons);
            AttachmentInventoryControl.Instance.UpdateAttachmentInventory(attachmentList);
            WeaponInformationControl.Instance.UpdateWeapon();
        }

        public List<Firearms> GetPlayerWeaponList()
        {
            var list = new List<Firearms>();
            for (int i = 0; i < playerWeapons.Length; i++)
            {
                if (playerWeapons[i] != null)
                {
                    list.Add(playerWeapons[i]);
                }
            }
            return list;
        }


        public Firearms DefaultWeapon { get { return defaultWeapon; } }
        public int MaxNumOfWeapons { get { return maxNumOfWeapons; } }
        public int MaxNumOfAttachmentsPerWeapon { get { return maxNumOfAttachmentsPerWeapon; } }
        public Firearms[] PlayerWeapons { get { return playerWeapons; } }
    }
}
