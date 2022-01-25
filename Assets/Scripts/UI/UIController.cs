using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private float fadingTime = 0.3f;
        [SerializeField] private GameObject ui;

        private void Start()
        {
            foreach (UI child in ui.GetComponentsInChildren<UI>())
                child.SetFadingTime(fadingTime);
        }
    }
}
