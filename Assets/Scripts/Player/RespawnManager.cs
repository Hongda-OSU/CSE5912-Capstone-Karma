using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class RespawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject respawnPoints;
        private List<RespawnPoint> respawnPointList;
        [SerializeField] private RespawnPoint currentRespawnPoint;

        private static RespawnManager instance;
        public static RespawnManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;
        }

        public void RespawnPlayerToLast()
        {
            PlayerManager.Instance.Player.transform.position = currentRespawnPoint.transform.position;
        }

        public RespawnPoint CurrentRespawnPoint { get { return currentRespawnPoint; } set { currentRespawnPoint = value; } }
    }
}
