using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class OpenMenuHandler
    {

        private IngameMenuController menuController;

        public OpenMenuHandler(InputAction action, IngameMenuController menuController)
        {
            action.performed += OpenMenu_performed;
            action.Enable();
            this.menuController = menuController;
        }

        void OpenMenu_performed(InputAction.CallbackContext obj)
        {
            menuController.SwitchActive();
        }
    }
}
