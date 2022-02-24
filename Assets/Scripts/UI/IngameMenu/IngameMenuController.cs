using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CSE5912.PolyGamers
{
    public class IngameMenuController : MonoBehaviour
    {

        private IngameMenu ingameMenu;

        bool isDisplayed = false;

        private static IngameMenuController instance;
        public static IngameMenuController Instance { get { return instance; } }


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;
        }

        private void Start()
        {
            ingameMenu = IngameMenu.Instance;
        }

        public void SwitchActive(InputActions inputSchemes)
        {
            if (!ingameMenu.GetComponent<IngameMenu>().IsFadingComplete)
                return;

            isDisplayed = !isDisplayed;

            WeaponsPanelControl.Instance.ResetPanel();

            PostProcessingController.Instance.SetBlurryCameraView(isDisplayed);

            if (isDisplayed)
            {
                inputSchemes.PlayerActions.Disable();
                inputSchemes.FPSActions.Disable();
                inputSchemes.UiActions.Enable();
            }
            else
            {
                inputSchemes.PlayerActions.Enable();
                inputSchemes.FPSActions.Enable();
            }

            StartCoroutine(ingameMenu.GetComponent<IngameMenu>().DisplayMenu(isDisplayed));
        }

    }
}
