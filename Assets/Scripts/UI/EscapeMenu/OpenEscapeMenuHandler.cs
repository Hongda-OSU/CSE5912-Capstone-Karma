using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class OpenEscapeMenuHandler
    {
        private InputActions inputSchemes;

        public OpenEscapeMenuHandler(InputActions inputSchemes)
        {
            this.inputSchemes = inputSchemes;

            inputSchemes.UiActions.EscapeMenu.performed += OpenMenu_performed;
            inputSchemes.UiActions.EscapeMenu.Enable();
        }

        void OpenMenu_performed(InputAction.CallbackContext obj)
        {
            if (IngameMenu.Instance.Root.style.display == DisplayStyle.Flex)
            {
                IngameMenuController.Instance.SwitchActive();
                return;
            }

            EscapeMenuController.Instance.SwitchActive();
        }
    }
}
