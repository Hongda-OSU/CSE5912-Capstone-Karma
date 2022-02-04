using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class IngameMenuController : MonoBehaviour
    {
        public IngameMenu ingameMenu;

        bool isDisplayed = false;

        private void Start()
        {
            
        }

        public void SwitchActive(InputActions inputSchemes)
        {
            isDisplayed = !isDisplayed;
            ingameMenu.GetComponent<UI>().SetDisplay(isDisplayed);

            if (isDisplayed)
            {
                inputSchemes.PlayerActions.Disable();
                inputSchemes.UiActions.Enable();
            }
            else
            {
                inputSchemes.PlayerActions.Enable();
            }
        }

    }
}
