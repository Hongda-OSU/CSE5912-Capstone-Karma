using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerManager : MonoBehaviour
    {

        public static PlayerManager instance;

        private void Awake()
        {
            instance = this;
        }

        public GameObject player;

    }
}