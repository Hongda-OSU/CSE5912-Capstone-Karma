using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class GameStateController : MonoBehaviour
    {
        public int karmicLevel = -1;

        [SerializeField] private GameState state;
        public float gamePlayTimeInSeconds;

        public Dictionary<string, bool> bossToAlive = new Dictionary<string, bool>();
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

            bossToAlive.Add("Golem", true);
            bossToAlive.Add("HellBlade", true);
            bossToAlive.Add("EvilGod", true);
            bossToAlive.Add("DarkSlayer", true);
            bossToAlive.Add("FailedHusk", true);

        }

        private void Start()
        {
            inputSchemes = InputManager.Instance.InputSchemes;
            SetGameState(GameState.InGame);
        }

        private void Update()
        {
            gamePlayTimeInSeconds += Time.unscaledDeltaTime;
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

        public void ResetBosses()
        {
            foreach (var kvp in bossToAlive)
                bossToAlive[kvp.Key] = true;
        }
        public GameState State { get { return state; } }
    }
}
