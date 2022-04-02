using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class WanderAreaManager : MonoBehaviour
    {
        public GetPoint Area_1;
        public GetPoint Area_2;
        public GetPoint Area_3;
        public GetPoint Area_4;

        public static WanderAreaManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public Vector3 GetRandomPoint(float AreaNumber) {
            GetPoint Instance = null;

            if (AreaNumber == 1f)
            {
                Instance = Area_1;
            }
            else if (AreaNumber == 2f)
            {
                Instance = Area_2;
            }
            else if (AreaNumber == 3f)
            {
                Instance = Area_3;
            }
            else if (AreaNumber == 4f)
            {
                Instance = Area_4;
            }
            else
            {
                Debug.LogWarning("Invalid Wander Range!");
            }

            return Instance.GetRandomPoint();
        }
    }
}
