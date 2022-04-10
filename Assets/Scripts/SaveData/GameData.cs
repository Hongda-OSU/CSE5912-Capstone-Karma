using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    [Serializable]
    [DataContract]
    public class GameData
    {
        [DataMember] public int sceneIndex;

        [DataMember] public string mapName;

        [DataMember] public float gamePlayTimeInSeconds;

        [DataMember] public float[] playerPosition = new float[3];

        [DataMember] public List<WeaponData> weaponDataList = new List<WeaponData>();

        [DataMember] public List<AttachmentData> attachmentDataList = new List<AttachmentData>();

        [DataMember] public PlayerStatsData playerStatsData;

        [DataMember] public PlayerSkillData playerSkillData;

        public GameData(List<Firearms> weaponList, List<Attachment> attachmentList)
        {
            sceneIndex = SceneManager.GetActiveScene().buildIndex;

            mapName = SceneManager.GetActiveScene().name;

            gamePlayTimeInSeconds = GameStateController.Instance.gamePlayTimeInSeconds;

            var player = PlayerManager.Instance.Player;

            playerPosition[0] = player.transform.position.x;
            playerPosition[1] = player.transform.position.y;
            playerPosition[2] = player.transform.position.z;

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
            playerSkillData = new PlayerSkillData(PlayerSkillManager.Instance);
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

            [DataMember] public Dictionary<string, int> nameToStatsLevel = new Dictionary<string, int>();

            public PlayerStatsData(PlayerStats stats)
            {
                statPoint = stats.StatPoint;
                level = stats.Level;
                experience = stats.Experience;
                experienceToUpgrade = stats.ExperienceToUpgrade;

                nameToStatsLevel = stats.NameToStatsLevel;
            }
        }


        [Serializable]
        [DataContract]
        public class PlayerSkillData
        {
            [DataMember] public int skillPoint;
            [DataMember] public Dictionary<string, int> skillToLevel = new Dictionary<string, int>();

            public PlayerSkillData(PlayerSkillManager skillManager)
            {
                skillPoint = skillManager.SkillPoints;
                
                for (int i = 0; i < skillManager.PlayerSkillList.Count; i++)
                {
                    var skill = skillManager.PlayerSkillList[i];

                    skillToLevel.Add(skill.Name, skill.Level);
                }
            }
        }

    }
}
