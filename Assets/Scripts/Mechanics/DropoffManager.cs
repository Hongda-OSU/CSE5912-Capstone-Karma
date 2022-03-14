using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class DropoffManager : MonoBehaviour
    {
        [SerializeField] private GameObject dropoffPrefabs;
        private List<GameObject> weaponDropoffList;
        private List<GameObject> attachmentDropoffList;

        [SerializeField] private GameObject baseWeaponPrefabs;
        private List<GameObject> baseWeaponList;

        [SerializeField] private Vector2 dropPositionVariance;

        private static DropoffManager instance;
        public static DropoffManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            baseWeaponList = new List<GameObject>();
            foreach (Transform weapon in baseWeaponPrefabs.transform)
            {
                baseWeaponList.Add(weapon.gameObject);
            }

            weaponDropoffList = new List<GameObject>();
            foreach (Transform drop in dropoffPrefabs.transform.Find("WeaponDropoffs").transform)
            {
                weaponDropoffList.Add(drop.gameObject);
            }

            attachmentDropoffList = new List<GameObject>();
            foreach (Transform drop in dropoffPrefabs.transform.Find("AttachmentDropoffs").transform)
            {
                attachmentDropoffList.Add(drop.gameObject);
            }
        }

        public GameObject DropWeapon(Firearms.WeaponType type, Firearms.WeaponRarity rarity, Vector3 position)
        {
            GameObject weaponObj = null;
            foreach (var baseWeapon in baseWeaponList)
            {
                if (baseWeapon.GetComponent<Firearms>().Type == type)
                {
                    weaponObj = Instantiate(baseWeapon);
                    weaponObj.transform.SetParent(transform);
                }
            }
            if (weaponObj == null)
            {
                Debug.LogError("Error: Weapon type " + type.ToString() + " not found. ");
                return null;
            }


            var weapon = weaponObj.GetComponent<Firearms>();

            weapon.Rarity = rarity;
            WeaponBonus weaponBonus = new WeaponBonus(weapon);
            weapon.Bonus = weaponBonus;

            weapon.gameObject.SetActive(false);


            GameObject dropoff = null;
            foreach (var drop in weaponDropoffList)
            {
                if (drop.GetComponent<FirearmsItem>().Type == type)
                {
                    Vector3 pos = position + Vector3.up * 2 * drop.GetComponent<Renderer>().bounds.size.y;

                    Vector3 offset = new Vector3(Random.Range(-dropPositionVariance.x, dropPositionVariance.x), 0, Random.Range(-dropPositionVariance.y, dropPositionVariance.y));
                    pos += offset;
                    dropoff = Instantiate(drop, pos, Quaternion.identity);
                    dropoff.transform.SetParent(transform);
                }
            }

            dropoff.GetComponent<FirearmsItem>().Setup(weapon);

            return dropoff;
        }

        public GameObject CreateDropoffFromWeapon(Firearms weapon, Vector3 position)
        {
            GameObject dropoff = null;
            foreach (var drop in weaponDropoffList)
            {
                if (drop.GetComponent<FirearmsItem>().Type == weapon.Type)
                {
                    dropoff = Instantiate(drop, position, Quaternion.identity);
                    dropoff.transform.SetParent(transform);
                }
            }
            
            dropoff.GetComponent<FirearmsItem>().Setup(weapon);

            return dropoff;
        }

        public GameObject DropAttachment(Attachment.AttachmentType type, Attachment.AttachmentRarity rarity, Vector3 position)
        {
            GameObject dropoff;

            var list = new List<GameObject>();
            foreach (var attachmentDropoff in attachmentDropoffList)
            {
                if (attachmentDropoff.GetComponent<AttachmentItem>().Type == type)
                {
                    list.Add(attachmentDropoff);
                }
            }

            var randomAttachment = list[Random.Range(0, list.Count)];

            Vector3 pos = position + Vector3.up * 6 * randomAttachment.GetComponent<Renderer>().bounds.size.y;

            dropoff = Instantiate(randomAttachment, pos, Quaternion.identity);
            dropoff.GetComponent<AttachmentItem>().Setup(rarity);

            dropoff.transform.SetParent(transform);
            dropoff.GetComponent<AttachmentItem>().Attachment.transform.SetParent(transform);

            return dropoff;
        }


        public void ClearDropoffs()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
