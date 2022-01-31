using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class MenuOpener : MonoBehaviour
    {
        [SerializeField] private IngameMenu ingameMenu;
        bool active = false;

        public void SwitchMenu()
        {
            active = !active;
            ingameMenu.GetComponent<UI>().SetActive(active);
            Debug.Log(active);
        }
    }
}
