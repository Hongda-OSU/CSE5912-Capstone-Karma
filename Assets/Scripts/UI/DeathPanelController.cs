using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class DeathPanelController : UI
    {

        private static DeathPanelController instance;
        public static DeathPanelController Instance { get { return instance; } }


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            Initialize();

            root.style.display = DisplayStyle.None;
        }


        public override void Display(bool enabled)
        {
            if (enabled)
            {
                if (IngameMenu.Instance.Root.style.display == DisplayStyle.Flex)
                {
                    IngameMenuController.Instance.SwitchActive();
                }
                if (EscapeMenu.Instance.Root.style.display == DisplayStyle.Flex)
                {
                    EscapeMenuController.Instance.SwitchActive();
                }
                PlayerHudPanelControl.Instance.Root.style.display = DisplayStyle.None;
                StartCoroutine(FadeIn(root));
            }
            else
            {
                PlayerHudPanelControl.Instance.Root.style.display = DisplayStyle.Flex;
                root.style.display = DisplayStyle.None;
            }
            EnemyHealthBarControl.Instance.DisplayEnemyHealthBars(!enabled);
        }
    }
}
