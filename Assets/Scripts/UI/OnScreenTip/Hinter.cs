using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    [RequireComponent(typeof(SphereCollider))]
    public class Hinter : MonoBehaviour
    {
        [TextArea(5, 10)]
        [SerializeField] private string text;

        private void Awake()
        {
            var collider3d = GetComponent<SphereCollider>();
            collider3d.enabled = true;
            collider3d.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (other.tag != "Player")
                return;

            OnScreenTipControl.Instance.Display(text);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag != "Player")
                return;

            OnScreenTipControl.Instance.Hide();
        }
    }
}
