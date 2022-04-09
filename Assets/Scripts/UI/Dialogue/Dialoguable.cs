using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    [RequireComponent(typeof(SphereCollider))]
    public class Dialoguable : MonoBehaviour
    {
        [TextArea(5, 10)]
        [SerializeField] private string text;

        private void Awake()
        {
            var collider3d = GetComponent<SphereCollider>();
            collider3d.enabled = true;
            collider3d.isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag != "Player")
                return;

            DialogueControl.Instance.Display(text);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag != "Player")
                return;

            DialogueControl.Instance.Hide();
        }
    }
}
