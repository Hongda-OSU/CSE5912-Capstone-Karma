using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using UnityEngine.SceneManagement;

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
                    LoadItems(saveData);
                }
            }
        }

        private void LoadItems(GameData data)
        {
            var weaponList = DropoffManager.Instance.BaseWeaponList;
            var weaponData = data.weaponData;

            foreach (Transform child in WeaponManager.Instance.WeaponCollection.transform)
            {
                PlayerInventory.Instance.RemoveWeapon(child.GetComponent<Firearms>());
                Destroy(child.gameObject);
            }

            GameObject weaponObj = null;
            foreach (var baseWeapon in weaponList)
            {
                if (baseWeapon.GetComponent<Firearms>().Type == weaponData.type)
                {
                    weaponObj = Instantiate(baseWeapon);
                    weaponObj.transform.SetParent(transform, false);
                }
            }
            if (weaponObj == null)
            {
                Debug.LogError("Error: Weapon type " + weaponData.type.ToString() + " not found. ");
                return;
            }

            var weapon = weaponObj.GetComponent<Firearms>();

            weapon.WeaponName = weaponData.name;
            weapon.Rarity = weaponData.rarity;
            weapon.Damage = weaponData.damage;
            weapon.Element = weaponData.element;
            weapon.CurrentAmmo = weaponData.currentAmmoInMag;
            weapon.CurrentMaxAmmoCarried = weaponData.currentTotalAmmo;
            weapon.Bonus = weaponData.weaponBonus;

            PlayerInventory.Instance.AddWeapon(weapon);

            weapon.gameObject.transform.SetParent(WeaponManager.Instance.WeaponCollection.transform, false);

            WeaponManager.Instance.SetupCarriedWeapon(weapon);
        }

        public void Save()
        {
            GameData gameData = new GameData(WeaponManager.Instance.CarriedWeapon);
            gameData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.persistentDataPath + "/" + fileName + ".savegame";
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, gameData);

            stream.Close();

            //Debug.Log(Application.persistentDataPath);

            //// Stream the file with a File Stream. (Note that File.Create() 'Creates' or 'Overwrites' a file.)
            //FileStream file = File.Create(Application.persistentDataPath + "/" + fileName + ".dat");

            //// Create a new Player_Data.
            //GameData gameData = new GameData(data, SceneManager.GetActiveScene().buildIndex);

            ////Serialize to xml
            //DataContractSerializer bf = new DataContractSerializer(gameData.GetType());
            //MemoryStream streamer = new MemoryStream();

            ////Serialize the file
            //bf.WriteObject(streamer, gameData);
            //streamer.Seek(0, SeekOrigin.Begin);

            ////Save to disk
            //file.Write(streamer.GetBuffer(), 0, streamer.GetBuffer().Length);

            //// Close the file to prevent any corruptions
            //file.Close();

            //string result = XElement.Parse(Encoding.ASCII.GetString(streamer.GetBuffer()).Replace("\0", "")).ToString();
            //Debug.Log("Serialized Result: " + result);

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
