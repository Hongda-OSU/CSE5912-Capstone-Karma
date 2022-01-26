using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private float fadingTime = 0.3f;
        [SerializeField] private GameObject uiObject;

        private List<UI> uiList;


        private void Awake()
        {
            uiList = new List<UI>();
            foreach (UI child in uiObject.GetComponentsInChildren<UI>())
                uiList.Add(child);
        }

        private void Start()
        {
            foreach (UI ui in uiList)
                ui.SetFadingTime(fadingTime);
        }
    }
}
