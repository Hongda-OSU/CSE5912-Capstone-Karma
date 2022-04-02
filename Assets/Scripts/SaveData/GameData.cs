using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    [Serializable]
    [DataContract]
    public class GameData
    {
        [DataMember] public int sceneIndex;

        [DataMember] public List<WeaponData> weaponDataList = new List<WeaponData>();

        [DataMember] public List<AttachmentData> attachmentDataList = new List<AttachmentData>();

        [DataMember] public PlayerStatsData playerStatsData;

        public GameData(List<Firearms> weaponList, List<Attachment> attachmentList)
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                weaponDataList.Add(new WeaponData(weaponList[i]));
            }

            for (int i = 0; i < attachmentList.Count; i++)
            {
                if (attachmentList[i].AttachedTo == null)
                {
                    attachmentDataList.Add(new AttachmentData(attachmentList[i]));
                }
            }

            playerStatsData = new PlayerStatsData(PlayerStats.Instance);
        }

        [Serializable]
        [DataContract]
        public class WeaponData
        {
            [DataMember] public string name;

            [DataMember] public Firearms.WeaponType type;

            [DataMember] public Firearms.WeaponRarity rarity;

            [DataMember] public float damage;

            [DataMember] public Element.Type element;

            [DataMember] public int currentAmmoInMag;

            [DataMember] public int currentTotalAmmo;

            [DataMember] public WeaponBonus weaponBonus;

            [DataMember] public List<AttachmentData> attachmentDataList = new List<AttachmentData>();

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

                for (int i = 0; i < weapon.Attachments.Length; i++)
                {
                    var attachment = weapon.Attachments[i];
                    if (attachment == null)
                        attachmentDataList.Add(null);
                    else 
                        attachmentDataList.Add(new AttachmentData(attachment));
                }
            }

        }

        [Serializable]
        [DataContract]
        public class AttachmentData
        {
            [DataMember] public string name;

            [DataMember] public string realName;

            [DataMember] public Attachment.AttachmentType type;

            [DataMember] public Attachment.AttachmentRarity rarity;

            [DataMember] public Attachment.AttachmentSet set;

            [DataMember] public AttachmentBonus attachmentBonus;

            [DataMember] public string citation;

            [DataMember] public string iconPath;

            public AttachmentData(Attachment attachment)
            {
                name = attachment.AttachmentName;
                realName = attachment.AttachmentRealName;
                type = attachment.Type;
                rarity = attachment.Rarity;
                set = attachment.Set;
                attachmentBonus = attachment.Bonus;
                citation = attachment.Citation;
                iconPath = AssetDatabase.GetAssetPath(attachment.IconImage);
            }
        }

        [Serializable]
        [DataContract]
        public class PlayerStatsData
        {
            [DataMember] public int statPoint;
            [DataMember] public int level;
            [DataMember] public float experience;
            [DataMember] public float experienceToUpgrade;
            //[DataMember] public float experienceMultiplier;

            [DataMember] public Dictionary<string, int> nameToStatsLevel = new Dictionary<string, int>();
            //[DataMember] public float health;
            //[DataMember] public float maxHealth;
            //[DataMember] public float energyShield;
            //[DataMember] public float maxEnergyShield;
            //[DataMember] public float armorShield;
            //[DataMember] public float maxArmorShield;
            //[DataMember] public float bulletVamp;
            //[DataMember] public float takeDamageFactor;

            //[DataMember] public float moveSpeedFactor;
            //[DataMember] public float reloadSpeedFactor;
            //[DataMember] public float fireRateFactor;

            //[DataMember] public float meleeDamageFactor;
            //[DataMember] public float meleeSpeedFactor;

            //[DataMember] public float critRate;
            //[DataMember] public float critDamageFactor;

            //[DataMember] public DamageFactor damageFactor;

            //[DataMember] public float burnedChance;
            //[DataMember] public float frozenChance;
            //[DataMember] public float electrocutedChance;
            //[DataMember] public float infectedChance;

            //[DataMember] public float burnedDamage;
            //[DataMember] public float frozenSlowdown;
            //[DataMember] public float electrocutedReduction;
            //[DataMember] public float infectedDamage;

            //[DataMember] public Resist resist;

            public PlayerStatsData(PlayerStats stats)
            {

                statPoint = stats.StatPoint;
                level = stats.Level;
                experience = stats.Experience;
                experienceToUpgrade = stats.ExperienceToUpgrade;

                nameToStatsLevel = stats.NameToStatsLevel;

                //experienceMultiplier = stats.ExperienceMultiplier;

                //health = stats.Health;
                //maxHealth = stats.MaxHealth;
                //energyShield = stats.Shield_energy;
                //maxEnergyShield = stats.MaxShield_energy;
                //armorShield = stats.Shield_armor;
                //maxArmorShield = stats.MaxShield_armor;
                //bulletVamp = stats.BulletVamp;
                //takeDamageFactor = stats.TakeDamageFactor;

                //moveSpeedFactor = stats.MoveSpeedFactor;
                //reloadSpeedFactor = stats.ReloadSpeedFactor;
                //fireRateFactor = stats.FireRateFactor;

                //meleeDamageFactor = stats.MeleeDamageFactor;
                //meleeSpeedFactor = stats.MeleeSpeedFactor;

                //critRate = stats.CritRate;
                //critDamageFactor = stats.CritDamageFactor;

                //damageFactor = stats.GetDamageFactor();

                //burnedChance = stats.BurnedBaseChance;
                //frozenChance = stats.FrozenBaseChance;
                //electrocutedChance = stats.ElectrocutedBaseChance;
                //infectedChance = stats.InfectedBaseChance;

                //burnedDamage = stats.BurnedDamagePerStack;
                //frozenSlowdown = stats.FrozenSlowdownPerStack;
                //electrocutedReduction = stats.ElectrocutedResistReductionPerStack;
                //infectedDamage = stats.InfectedCurrentHealthDamagePerStack;

                //resist = stats.GetResist();
            }
        }

    }
}
