using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEditor;

namespace CSE5912.PolyGamers
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField] private string fileName = "GameData";

        [SerializeField] private static int currentDataIndex = -1;

        [SerializeField] private bool isSceneLoaded = true;

        private static DataManager instance;
        public static DataManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            //// test
            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    var index = 99;
            //    currentDataIndex = index;

            //    var saveData = Load(index);
            //    if (saveData != null)
            //    {
            //        LoadDataToGame();
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.O))
            //    Save();

            if (!isSceneLoaded && SceneManager.GetActiveScene().buildIndex != 0)
            {
                isSceneLoaded = true;
                LoadDataToGame();
            }
        }

        public void LoadSaveData(int dataIndex)
        {
            isSceneLoaded = false;
            currentDataIndex = dataIndex;
        }
        public void LoadDataToGame()
        {
            var data = Load(currentDataIndex);

            if (data == null)
                return;

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

            // load position
            var player = PlayerManager.Instance.Player;

            var position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            FPSControllerCC.Instance.CharacterController.enabled = false;
            player.transform.position = position;
            FPSControllerCC.Instance.CharacterController.enabled = true;

            // load playtime
            GameStateController.Instance.gamePlayTimeInSeconds = data.gamePlayTimeInSeconds;

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
                var skill = PlayerSkillManager.Instance.GetPlayerSkill(kvp.Key);

                skill.SetLevel(kvp.Value);
                SkillsPanelControl.Instance.CurrentSkillTree.FindSlotBySkill(skill).UpdateVisual();
            }
        }

        public void Save()
        {
            var inventory = PlayerInventory.Instance;

            GameData gameData = new GameData(inventory.GetPlayerWeaponList(), inventory.AttachmentList);

            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.persistentDataPath + "/" + fileName + "_" + currentDataIndex + ".savegame";
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, gameData);

            stream.Close();

            Debug.Log("Data saved. Path: " + path);
        }

        public GameData Load(int index)
        {
            string path = Application.persistentDataPath + "/" + fileName + "_" + index + ".savegame";

            if (File.Exists(path))
            {
                Debug.Log("Save data found. ");

                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                try
                {
                    GameData gameData = formatter.Deserialize(stream) as GameData;
                    stream.Close();

                    Debug.Log("Save data loaded. Path: " + path);

                    return gameData;
                }
                catch (Exception)
                {
                    stream.Close();
                    Debug.LogError("Failed to load save data. Path: " + path);

                    return null;
                }

            }
            else
            {
                Debug.Log("Save data not found. Path: " + path);

                return null;
            }
        }

        public void Clear(int index)
        {
            string path = Application.persistentDataPath + "/" + fileName + "_" + index + ".savegame";

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    Debug.Log("Save data deleted. Path: " + path);
                }
                catch (Exception)
                {
                    Debug.LogError("Failed to delete save data. Path: " + path);
                }

            }
            else
            {
                Debug.Log("Save data not found. Path: " + path);
            }
        }

    }

}
