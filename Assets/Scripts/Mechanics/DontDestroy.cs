using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    public class DontDestroy : MonoBehaviour
    {
        [SerializeField] private GameObject[] objects;


        private static DontDestroy instance;
        public static DontDestroy Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            foreach (var obj in objects)
            {
                DontDestroyOnLoad(obj);
            }
        }

        public void Destroy()
        {
            foreach (var obj in objects)
                SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
        }
    }
}
