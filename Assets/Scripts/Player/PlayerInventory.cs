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

        [SerializeField] private GameObject attachmentParent;

        private WeaponsPanelControl weaponsPanelControl;
        private AttachmentInventoryControl attachmentInventoryControl;

        [SerializeField] private Firearms[] playerWeapons; 

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

            playerWeapons = new Firearms[maxNumOfWeapons];
            attachmentList = new List<Attachment>();
        }

        private void Start()
        {
            weaponsPanelControl = WeaponsPanelControl.Instance;
            attachmentInventoryControl = AttachmentInventoryControl.Instance;
            AddWeapon(defaultWeapon);
        }

        private void Update()
        {

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

        public void RemoveWeapon(Firearms weapon)
        {
            for (int i = 0; i < playerWeapons.Length; i++)
            {
                if (playerWeapons[i] == weapon)
                {
                    playerWeapons[i] = null;
                    break;
                }
            }
            UpdateAll();
        }

        public void AddAttachment(Attachment attachment)
        {
            attachmentList.Add(attachment);
            attachment.transform.SetParent(attachmentParent.transform);

            UpdateAll();
        }

        public void RemoveAttachment(Attachment attachment)
        {
            attachmentList.Remove(attachment);
            Destroy(attachment.gameObject);

            UpdateAll();
        }

        public bool IsWeaponInventoryFull()
        {
            foreach (var weapon in playerWeapons)
                if (weapon == null)
                    return false;
            return true;
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
