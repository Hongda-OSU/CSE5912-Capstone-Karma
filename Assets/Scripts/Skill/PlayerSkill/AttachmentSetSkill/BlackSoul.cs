using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class BlackSoul : PlayerSkill
    {
        [Header("Black Soul")]
        [SerializeField] private GameObject blackSoulPointPrefab;
        [SerializeField] private float statUp = 0.2f;
        [SerializeField] private float duration = 30f;

        private GameObject originSoulPoint;

        protected override string GetBuiltSpecific()
        {
            return "";
        }

        private void Start()
        {
            originSoulPoint = RespawnManager.Instance.soulPointPrefab;
        }
        public override bool LevelUp()
        {
            var result = base.LevelUp();

            if (result)
            {
                //originSoulPoint = RespawnManager.Instance.soulPointPrefab;

                blackSoulPointPrefab.GetComponent<BlackSoulPoint>().statUp = statUp;
                blackSoulPointPrefab.GetComponent<BlackSoulPoint>().duration = duration;
                RespawnManager.Instance.soulPointPrefab = blackSoulPointPrefab;
            }

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            RespawnManager.Instance.soulPointPrefab = originSoulPoint;
        }
    }
}
