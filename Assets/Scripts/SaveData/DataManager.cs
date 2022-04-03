using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;

namespace CSE5912.PolyGamers
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField] private string fileName = "GameData";


        private static DataManager instance;
        public static DataManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;
        }

        private void Update()
        {
            // test
            if (Input.GetKeyDown(KeyCode.P))
            {
                var saveData = Load();
                if (saveData != null)
                {
                    LoadData(saveData);
                }
            }
            if (Input.GetKeyDown(KeyCode.O))
                Save();
        }

        private void LoadData(GameData data)
        {
            // clear attachments
            foreach (Transform child in PlayerInventory.Instance.AttachmentCollection.transform)
            {
                Destroy(child.gameObject);
            }
            PlayerInventory.Instance.AttachmentList.Clear();

            // clear current weapons
            foreach (Transform child in WeaponManager.Instance.WeaponCollection.transform)
            {
                PlayerInventory.Instance.RemoveWeapon(child.GetComponent<Firearms>());
                Destroy(child.gameObject);
            }


            // load player stats
            LoadPlayerStats(data.playerStatsData);

            // load player skills
            LoadPlayerSkill(data.playerSkillData);

            // load weapon data
            for (int i = 0; i < data.weaponDataList.Count; i++)
            {
                var weaponData = data.weaponDataList[i];
                var weapon = LoadWeapon(weaponData);

                // load attached attachments
                for (int j = 0; j < weaponData.attachmentDataList.Count; j++)
                {
                    if (weaponData.attachmentDataList[j] == null)
                        continue;
                    var attachment = LoadAttachment(weaponData.attachmentDataList[j]);
                    PlayerInventory.Instance.AddAttachment(attachment);
                    weapon.SetAttachment(attachment, j);
                }
            }
            WeaponManager.Instance.SetupCarriedWeapon(PlayerInventory.Instance.GetPlayerWeaponList()[0]);
            PlayerSkillManager.Instance.TryActivateSetSkill();

            // load attachments
            for (int i = 0; i < data.attachmentDataList.Count; i++)
            {
                var attachment = LoadAttachment(data.attachmentDataList[i]);
                PlayerInventory.Instance.AddAttachment(attachment);
            }

        }

        private Firearms LoadWeapon(GameData.WeaponData data)
        {
            var weaponList = DropoffManager.Instance.BaseWeaponList;
            // create base weapon of saved type
            GameObject weaponObj = null;
            foreach (var baseWeapon in weaponList)
            {
                if (baseWeapon.GetComponent<Firearms>().Type == data.type)
                {
                    weaponObj = Instantiate(baseWeapon);
                    weaponObj.transform.SetParent(transform, false);
                }
            }
            if (weaponObj == null)
            {
                Debug.LogError("Error: Weapon type " + data.type.ToString() + " not found. ");
                return null;
            }

            // load saved weapon data
            var weapon = weaponObj.GetComponent<Firearms>();
            weapon.Attachments = new Attachment[4];
            weapon.PerformBonus(false);

            weapon.WeaponName = data.name;
            weapon.Rarity = data.rarity;
            weapon.Damage = data.damage;
            weapon.Element = data.element;
            weapon.CurrentAmmo = data.currentAmmoInMag;
            weapon.CurrentMaxAmmoCarried = data.currentTotalAmmo;
            weapon.Bonus = data.weaponBonus;

            weapon.Bonus.SetBonusReady(true);

            // final setup
            PlayerInventory.Instance.AddWeapon(weapon);
            weapon.gameObject.transform.SetParent(WeaponManager.Instance.WeaponCollection.transform, false);
            weapon.gameObject.SetActive(false);

            return weapon;
        }

        private Attachment LoadAttachment(GameData.AttachmentData data)
        {
            GameObject attachmentObj = new GameObject();
            attachmentObj.transform.SetParent(PlayerInventory.Instance.AttachmentCollection.transform, false);

            var attachment = attachmentObj.AddComponent<Attachment>();

            attachment.AttachmentName = data.name;
            attachment.AttachmentRealName = data.realName;
            attachment.Type = data.type;
            attachment.Rarity = data.rarity;
            attachment.Set = data.set;
            attachment.SetSkill = PlayerSkillManager.Instance.GetSetSkill(attachment.Set);
            attachment.Bonus = data.attachmentBonus;
            attachment.Citation = data.citation;

            var iconImage = AssetDatabase.LoadAssetAtPath<Sprite>(data.iconPath);
            if (iconImage == null)
                Debug.LogError("Image path not found: " + data.iconPath);
            attachment.IconImage = iconImage;

            attachment.Bonus.SetBonusReady(true);
            //attachment.PerformBonus(false);

            attachment.gameObject.name = attachment.AttachmentName;

            return attachment;
        }

        private void LoadPlayerStats(GameData.PlayerStatsData data)
        {
            var stats = PlayerStats.Instance;

            stats.ResetStats();

            stats.StatPoint = data.statPoint;
            stats.Level = data.level;
            stats.Experience = data.experience;
            stats.ExperienceToUpgrade = data.experienceToUpgrade;

            foreach (var kvp in data.nameToStatsLevel)
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    stats.AddStat(kvp.Key, true);
                }
            }
            stats.NameToStatsLevel = data.nameToStatsLevel;

            //PlayerStats.Instance.LoadPlayerStatsData(data);
        }

        private void LoadPlayerSkill(GameData.PlayerSkillData data)
        {
            var skillManager = PlayerSkillManager.Instance;

            skillManager.SkillPoints = data.skillPoint;

            foreach (var kvp in data.skillToLevel)
            {
                kvp.Key.SetLevel(kvp.Value);
            }
        }

        public void Save()
        {
            var inventory = PlayerInventory.Instance;

            GameData gameData = new GameData(inventory.GetPlayerWeaponList(), inventory.AttachmentList);
            gameData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.persistentDataPath + "/" + fileName + ".savegame";
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, gameData);

            stream.Close();

            Debug.Log("Data saved. ");
        }

        public GameData Load()
        {
            string path = Application.persistentDataPath + "/" + fileName + ".savegame";

            if (File.Exists(path))
            {
                Debug.Log("Save data found. ");

                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                try
                {
                    GameData gameData = formatter.Deserialize(stream) as GameData;
                    stream.Close();

                    Debug.Log("Save data loaded. ");

                    return gameData;
                }
                catch (Exception)
                {
                    stream.Close();
                    Debug.LogError("Failed to load save data. ");

                    return null;
                }

            }
            else
            {
                Debug.Log("Save data not found. ");

                return null;
            }
        }

    }

}
