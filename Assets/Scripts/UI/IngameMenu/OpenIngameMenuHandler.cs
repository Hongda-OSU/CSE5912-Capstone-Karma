using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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

            IngameMenuController.Instance.Initialize(inputSchemes);
        }

        void OpenMenu_performed(InputAction.CallbackContext obj)
        {
            if (EscapeMenu.Instance.Root.style.display == DisplayStyle.Flex)
                return;

            IngameMenuController.Instance.SwitchActive();
        }
    }
}
