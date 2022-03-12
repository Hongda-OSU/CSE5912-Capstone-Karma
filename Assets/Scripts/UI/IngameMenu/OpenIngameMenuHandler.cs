using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class OpenIngameMenuHandler
    {
        private InputActions inputSchemes;

        public OpenIngameMenuHandler(InputActions inputSchemes)
        {
            this.inputSchemes = inputSchemes;

            inputSchemes.UiActions.IngameMenu.performed += OpenMenu_performed;
            inputSchemes.UiActions.IngameMenu.Enable();
        }

        void OpenMenu_performed(InputAction.CallbackContext obj)
        {
            IngameMenuController.Instance.SwitchActive(inputSchemes);
        }
    }
}
