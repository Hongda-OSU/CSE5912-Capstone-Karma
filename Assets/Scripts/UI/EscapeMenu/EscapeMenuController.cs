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
            if (isDisplayed)
                IngameAudioControl.Instance.SmoothMusicVolume(0.3f);
            else
                IngameAudioControl.Instance.SmoothMusicVolume(1f);

            PostProcessingController.Instance.SetBlurryCameraView(isDisplayed);

            if (isDisplayed)
            {
                GameStateController.Instance.SetGameState(GameStateController.GameState.InMenu);

                PlayerHudPanelControl.Instance.Root.style.display = DisplayStyle.None;
            }
            else
            {
                GameStateController.Instance.SetGameState(GameStateController.GameState.InGame);

                PlayerHudPanelControl.Instance.Root.style.display = DisplayStyle.Flex;
            }
            EnemyHealthBarControl.Instance.DisplayEnemyHealthBars(!isDisplayed);

            StartCoroutine(escapeMenu.GetComponent<EscapeMenu>().DisplayMenu(isDisplayed));
        }

    }
}
