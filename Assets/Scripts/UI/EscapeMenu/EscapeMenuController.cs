using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class EscapeMenuController : MonoBehaviour
    {

        private EscapeMenu escapeMenu;

        public bool isDisplayed = false;

        private static EscapeMenuController instance;
        public static EscapeMenuController Instance { get { return instance; } }


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
            escapeMenu = EscapeMenu.Instance;
        }

        public void SwitchActive(InputActions inputSchemes)
        {
            if (!escapeMenu.GetComponent<EscapeMenu>().IsFadingComplete)
                return;

            isDisplayed = !isDisplayed;

            PostProcessingController.Instance.SetBlurryCameraView(isDisplayed);

            if (isDisplayed)
            {
                inputSchemes.PlayerActions.Disable();
                inputSchemes.FPSActions.Disable();
                inputSchemes.UiActions.Enable();

                PlayerHudPanelControl.Instance.Root.style.display = DisplayStyle.None;
            }
            else
            {
                inputSchemes.PlayerActions.Enable();
                inputSchemes.FPSActions.Enable();

                PlayerHudPanelControl.Instance.Root.style.display = DisplayStyle.Flex;
            }
            EnemyHealthBarControl.Instance.DisplayEnemyHealthBars(!isDisplayed);

            StartCoroutine(escapeMenu.GetComponent<EscapeMenu>().DisplayMenu(isDisplayed));
        }

    }
}
