using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{

    public class MenuOpenHandler
    {
        private MenuOpener menuOpener;

        public MenuOpenHandler(InputAction action, MenuOpener menuOpener)
        {
            action.performed += OpenMenu_performed;
            action.Enable();
            this.menuOpener = menuOpener;
        }

        void OpenMenu_performed(InputAction.CallbackContext obj)
        {
            menuOpener.SwitchMenu();
        }
    }
}
