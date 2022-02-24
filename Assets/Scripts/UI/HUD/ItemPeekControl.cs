using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class ItemPeekControl : UI
    {
        private VisualElement weaponCurrentPanel;
        private VisualElement weaponPeekPanel;

        private VisualElement attachmentPeekPanel;

        private BaseItem prevItem;

        private static ItemPeekControl instance;
        public static ItemPeekControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            weaponCurrentPanel = root.Q<VisualElement>("WeaponPeek").Q<VisualElement>("Current");
            weaponPeekPanel = root.Q<VisualElement>("WeaponPeek").Q<VisualElement>("Peek");
            attachmentPeekPanel = root.Q<VisualElement>("AttachmentPeek");
        }

        public void PeekItem(BaseItem item)
        {
            if (item == prevItem)
                return;

            Clear();
            if (item is FirearmsItem firearmsItem)
            {
                PeekWeapon(firearmsItem.Weapon);
            }
            else if (item is AttachmentItem attachmentItem)
            {
                PeekAttachment(attachmentItem.Attachment);
            }

            prevItem = item;
        }

        private void PeekWeapon(Firearms weapon)
        {
            root.Q<VisualElement>("WeaponPeek").style.display = DisplayStyle.Flex;

            StylizePanel(weaponCurrentPanel, WeaponManager.Instance.CarriedWeapon);
            StylizePanel(weaponPeekPanel, weapon);
        }

        private void PeekAttachment(Attachment attachment)
        {

        }

        public void Clear()
        {
            prevItem = null;

            root.Q<VisualElement>("WeaponPeek").style.display = DisplayStyle.None;
            attachmentPeekPanel.style.display = DisplayStyle.None;
        }

        private void StylizePanel(VisualElement panel, Firearms weapon)
        {
            Color color = WeaponsPanelControl.Instance.WeaponRarityToColor[weapon.Rarity];

            panel.style.borderBottomColor = color;
            panel.style.borderLeftColor = color;
            panel.style.borderRightColor = color;


            VisualElement title = panel.Q<VisualElement>("Title");
            VisualElement specific = panel.Q<VisualElement>("Specific");
            VisualElement bonus = panel.Q<VisualElement>("Bonus");


            title.Q<Label>("Name").text = weapon.WeaponName;
            title.Q<Label>("Name").style.color = color;

            title.Q<Label>("Type").text = weapon.Type.ToString();


            //specific.Q<VisualElement>("Rarity").Q<Label>("Label").style.color = rarityToColor[weapon.Rarity];
            specific.Q<VisualElement>("Rarity").Q<Label>("Data").text = weapon.Rarity.ToString();
            specific.Q<VisualElement>("Rarity").Q<Label>("Data").style.color = color;

            //specific.Q<VisualElement>("Damage").Q<Label>("Label").style.color = rarityToColor[weapon.Rarity];
            specific.Q<VisualElement>("Damage").Q<Label>("Data").text = weapon.Damage.ToString();

            //specific.Q<VisualElement>("Element").Q<Label>("Label").style.color = rarityToColor[weapon.Rarity];
            specific.Q<VisualElement>("Element").Q<Label>("Data").text = weapon.Element.ToString();
            specific.Q<VisualElement>("Element").Q<Label>("Data").style.color = Element.Instance.TypeToColor[weapon.Element];

            //specific.Q<VisualElement>("Ammo").Q<Label>("Label").style.color = rarityToColor[weapon.Rarity];
            specific.Q<VisualElement>("Ammo").Q<Label>("Data").text = weapon.AmmoInMag.ToString();


            var list = weapon.Bonus.GetBonusDescriptionList();
            int num = 0;
            foreach (var child in bonus.Children())
            {
                if (num < list.Count)
                {
                    child.Q<Label>("Data").text = list[num];
                    child.style.display = DisplayStyle.Flex;
                }
                else
                {
                    child.style.display = DisplayStyle.None;
                }
                num++;
            }

            panel.style.opacity = 1f;
            panel.style.display = DisplayStyle.Flex;
        }
    }
}
