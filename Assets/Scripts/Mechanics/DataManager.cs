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
        [SerializeField] private GameObject data;

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
                    WeaponManager.Instance.CarriedWeapon.Bonus = saveData.weaponBonus;
                }
            }
        }
        public void Save()
        {
            GameData gameData = new GameData();
            gameData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            gameData.weaponBonus = WeaponManager.Instance.CarriedWeapon.Bonus;

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

    [Serializable]
    [DataContract]
    public class GameData
    {
        [DataMember]
        public int sceneIndex;

        [DataMember]
        public WeaponBonus weaponBonus;


    }
}
