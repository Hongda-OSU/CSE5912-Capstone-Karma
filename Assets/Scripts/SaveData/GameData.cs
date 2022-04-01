using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    [Serializable]
    [DataContract]
    public class GameData
    {
        [DataMember]
        public int sceneIndex;

        [DataMember]
        public List<WeaponData> weaponDataList = new List<WeaponData>();

        public GameData(List<Firearms> weaponList)
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                weaponDataList.Add(new WeaponData(weaponList[i]));
            }
        }

        [Serializable]
        [DataContract]
        public class WeaponData
        {
            [DataMember]
            public string name;

            [DataMember]
            public Firearms.WeaponType type;

            [DataMember]
            public Firearms.WeaponRarity rarity;

            [DataMember]
            public float damage;

            [DataMember]
            public Element.Type element;

            [DataMember]
            public int currentAmmoInMag;
            [DataMember]
            public int currentTotalAmmo;

            [DataMember]
            public WeaponBonus weaponBonus;

            //[DataMember]
            //attachment

            public WeaponData(Firearms weapon)
            {
                name = weapon.WeaponName;
                type = weapon.Type;
                rarity = weapon.Rarity;
                damage = weapon.Damage;
                element = weapon.Element;
                currentAmmoInMag = weapon.CurrentAmmo;
                currentTotalAmmo = weapon.CurrentMaxAmmoCarried;
                weaponBonus = weapon.Bonus;
            }

        }
    }
}
