using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class GameStateController : MonoBehaviour
    {
        [SerializeField] private GameState state;

        private InputActions inputSchemes;

        private static GameStateController instance;
        public static GameStateController Instance { get { return instance; } }
        public enum GameState
        {
            InGame,
            InMenu,
            Loading,
            GameOver,
        }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;
        }

        public void Initialize(InputActions inputSchemes)
        {
            this.inputSchemes = inputSchemes;
        }

        public void SetGameState(GameState state)
        {
            switch (state)
            {
                case GameState.InGame:
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                    inputSchemes.PlayerActions.Enable();
                    inputSchemes.FPSActions.Enable();
                    inputSchemes.UiActions.Enable();

                    break;
                case GameState.InMenu:
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    inputSchemes.PlayerActions.Disable();
                    inputSchemes.FPSActions.Disable();
                    inputSchemes.UiActions.Enable();

                    break;

                case GameState.Loading:
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                    inputSchemes.PlayerActions.Disable();
                    inputSchemes.FPSActions.Disable();
                    inputSchemes.UiActions.Disable();
                    break;

                case GameState.GameOver:
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    inputSchemes.PlayerActions.Disable();
                    inputSchemes.FPSActions.Disable();
                    inputSchemes.UiActions.Disable();
                    break;
            }
        }
        public GameState State { get { return state; } }
    }
}
