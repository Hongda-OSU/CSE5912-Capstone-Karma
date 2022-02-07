using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class ScrollHandler
    {
        private WeaponsPanelControl weaponsPanelControl;

        private InputActions inputSchemes;

        public ScrollHandler(InputActions inputSchemes, WeaponsPanelControl weaponsPanelControl)
        {
            this.inputSchemes = inputSchemes;

            inputSchemes.UiActions.Scroll.performed += ScrollHandler_performed;
            inputSchemes.UiActions.Scroll.Enable();
            this.weaponsPanelControl = weaponsPanelControl;
        }

        void ScrollHandler_performed(InputAction.CallbackContext obj)
        {
            float scroll = obj.ReadValue<float>();
            int step = 0;
            if (scroll < 0)
                step = 1;
            else if (scroll > 0)
                step = -1;

            weaponsPanelControl.attachmentInventoryControl.FlipInventoryPage(step);
            weaponsPanelControl.UpdateSlotsVisual();
        }
    }
}
