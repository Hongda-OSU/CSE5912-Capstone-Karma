using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class PlayerHudPanelControl : UI
    {
        private static PlayerHudPanelControl instance;
        public static PlayerHudPanelControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this) 
                Destroy(gameObject);
            instance = this;

            Initialize();

            root.Q<VisualElement>("Background").style.display = DisplayStyle.Flex;
        }

        private void Start()
        {
            CrosshairContorl.Instance.Display(true);
            PlayerHealthBarControl.Instance.Display(true);
            WeaponInformationControl.Instance.Display(true);
            ItemPeekControl.Instance.Display(false);
        }

    }
}
