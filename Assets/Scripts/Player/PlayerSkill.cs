using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerSkill : MonoBehaviour
    {

        public int skillPoints = 0;


        private static PlayerSkill instance;
        public static PlayerSkill Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;
        }

    }
}
