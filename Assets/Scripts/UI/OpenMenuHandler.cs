using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class OpenMenuHandler
    {
        private IngameMenuController menuController;

        private InputActions inputSchemes;

        public OpenMenuHandler(InputActions inputSchemes, IngameMenuController menuController)
        {
            this.inputSchemes = inputSchemes;

            inputSchemes.UiActions.OpenMenu.performed += OpenMenu_performed;
            inputSchemes.UiActions.OpenMenu.Enable();
            this.menuController = menuController;
        }

        void OpenMenu_performed(InputAction.CallbackContext obj)
        {
            menuController.SwitchActive(inputSchemes);
        }
    }
}
