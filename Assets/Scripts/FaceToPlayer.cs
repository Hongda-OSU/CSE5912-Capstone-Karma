using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FaceToPlayer : MonoBehaviour
    {
        private void Update()
        {
            gameObject.GetComponent<Transform>().rotation = Quaternion.LookRotation(gameObject.transform.position - PlayerManager.Instance.PlayerCamera.transform.position);
        }
    }
}
