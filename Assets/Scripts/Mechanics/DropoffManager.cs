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

        [SerializeField] private float dropHeight;
        [SerializeField] private Vector2 dropPositionVariance;

        [SerializeField] private GameObject bulletEffects;
        private List<GameObject> cryoList;
        private List<GameObject> electroList;
        private List<GameObject> fireList;
        private List<GameObject> physicalList;
        private List<GameObject> venomList;

        private static DropoffManager instance;
        public static DropoffManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

        }
        private void Start()
        {
            var preInstWeapon = Instantiate(baseWeaponPrefabs);
            Destroy(preInstWeapon);

            var preInstDropoff = Instantiate(dropoffPrefabs);
            Destroy(preInstDropoff);

            var preInstBullet = Instantiate(bulletEffects);
            Destroy(preInstBullet);

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

            cryoList = new List<GameObject>();
            foreach (Transform effect in bulletEffects.transform.Find("Cryo").transform)
            {
                cryoList.Add(effect.gameObject);
            }

            electroList = new List<GameObject>();
            foreach (Transform effect in bulletEffects.transform.Find("Electro").transform)
            {
                electroList.Add(effect.gameObject);
            }

            fireList = new List<GameObject>();
            foreach (Transform effect in bulletEffects.transform.Find("Fire").transform)
            {
                fireList.Add(effect.gameObject);
            }

            physicalList = new List<GameObject>();
            foreach (Transform effect in bulletEffects.transform.Find("Physical").transform)
            {
                physicalList.Add(effect.gameObject);
            }

            venomList = new List<GameObject>();
            foreach (Transform effect in bulletEffects.transform.Find("Venom").transform)
            {
                venomList.Add(effect.gameObject);
            }
        }

        public GameObject DropWeapon(Firearms.WeaponType type, Firearms.WeaponRarity rarity, Vector3 position)
        {
            GameObject weaponObj = null;
            foreach (var baseWeapon in baseWeaponList)
            {
                if (baseWeapon.GetComponent<Firearms>().Type == type)
                {
                    weaponObj = Instantiate(baseWeapon) as GameObject;
                    weaponObj.transform.SetParent(transform, false);
                }
            }
            if (weaponObj == null)
            {
                Debug.LogError("Error: Weapon type " + type.ToString() + " not found. ");
                return null;
            }

            // awake > onenable > assign
            var weapon = weaponObj.GetComponent<Firearms>();

            weapon.Rarity = rarity;
            weapon.PerformBonus(false);
            WeaponBonus weaponBonus = new WeaponBonus(weapon);
            weapon.Bonus = weaponBonus;
            weapon.Element = (Element.Type)Random.Range(0, 5);
            if (weapon.Element is Element.Type.Cryo)
            {
                weapon.BulletPrefab = cryoList[0];
                weapon.ImpactPrefab = cryoList[1];
                GameObject muzzleParticle = Instantiate(cryoList[2], weapon.transform.Find("Armature/weapon/Components/MuzzleFlareHolder"), false);
                weapon.MuzzleParticle = muzzleParticle.GetComponent<ParticleSystem>();
            }
            else if (weapon.Element is Element.Type.Electro)
            {
                weapon.BulletPrefab = electroList[0];
                weapon.ImpactPrefab = electroList[1];
                GameObject muzzleParticle = Instantiate(electroList[2], weapon.transform.Find("Armature/weapon/Components/MuzzleFlareHolder"), false);
                weapon.MuzzleParticle = muzzleParticle.GetComponent<ParticleSystem>();
            }
            else if (weapon.Element is Element.Type.Fire)
            {
                weapon.BulletPrefab = fireList[0];
                weapon.ImpactPrefab = fireList[1];
                GameObject muzzleParticle = Instantiate(fireList[2], weapon.transform.Find("Armature/weapon/Components/MuzzleFlareHolder"), false);
                weapon.MuzzleParticle = muzzleParticle.GetComponent<ParticleSystem>();
            }
            else if (weapon.Element is Element.Type.Physical)
            {
                weapon.BulletPrefab = physicalList[0];
                weapon.ImpactPrefab = physicalList[1];
                GameObject muzzleParticle = Instantiate(physicalList[2], weapon.transform.Find("Armature/weapon/Components/MuzzleFlareHolder"), false);
                weapon.MuzzleParticle = muzzleParticle.GetComponent<ParticleSystem>();
            }
            else if (weapon.Element is Element.Type.Venom)
            {
                weapon.BulletPrefab = venomList[0];
                weapon.ImpactPrefab = venomList[1];
                GameObject muzzleParticle = Instantiate(venomList[2], weapon.transform.Find("Armature/weapon/Components/MuzzleFlareHolder"), false);
                weapon.MuzzleParticle = muzzleParticle.GetComponent<ParticleSystem>();
            }

            weapon.gameObject.SetActive(false);


            GameObject dropoff = null;
            foreach (var drop in weaponDropoffList)
            {
                if (drop.GetComponent<FirearmsItem>().Type == type)
                {
                    Vector3 pos = position + Vector3.up * dropHeight;

                    Vector3 offset = new Vector3(Random.Range(-dropPositionVariance.x, dropPositionVariance.x), 0, Random.Range(-dropPositionVariance.y, dropPositionVariance.y));
                    pos += offset;
                    dropoff = Instantiate(drop, pos, Quaternion.identity);
                    dropoff.transform.SetParent(transform, true);
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
                    dropoff.transform.SetParent(transform, true);
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

            Vector3 pos = position + Vector3.up * dropHeight;
            Vector3 offset = new Vector3(Random.Range(-dropPositionVariance.x, dropPositionVariance.x), 0, Random.Range(-dropPositionVariance.y, dropPositionVariance.y));
            pos += offset;

            dropoff = Instantiate(randomAttachment, pos, Quaternion.identity);
            dropoff.GetComponent<AttachmentItem>().Setup(rarity);

            dropoff.transform.SetParent(transform, true);
            dropoff.GetComponent<AttachmentItem>().Attachment.transform.SetParent(transform, true);

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
