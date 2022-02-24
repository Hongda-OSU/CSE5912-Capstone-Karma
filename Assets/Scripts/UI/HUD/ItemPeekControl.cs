using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class ItemPeekControl : UI
    {
        [SerializeField] private float peekScale = 1f;
        [SerializeField] private float xOffset = 0f;
        [SerializeField] private float yOffset = 0f;

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

            weaponPeekPanel = root.Q<VisualElement>("WeaponPeek");
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
            weaponPeekPanel.style.display = DisplayStyle.Flex;

            weaponPeekPanel.style.scale = new Scale(Vector3.one * peekScale);

            StylizePanel(weaponPeekPanel, weapon);

            SetPosition(weaponPeekPanel);
        }

        private void PeekAttachment(Attachment attachment)
        {
            attachmentPeekPanel.style.display = DisplayStyle.Flex;

            attachmentPeekPanel.style.scale = new Scale(Vector3.one * peekScale);

            StylizePanel(attachmentPeekPanel, attachment);

            SetPosition(attachmentPeekPanel);
        }

        public void Clear()
        {
            prevItem = null;

            root.Q<VisualElement>("WeaponPeek").style.display = DisplayStyle.None;
            attachmentPeekPanel.style.display = DisplayStyle.None;
        }

        private void SetPosition(VisualElement element)
        {
            element.transform.position = new Vector2(Screen.width / 2 + xOffset, Screen.height / 2 + yOffset);
        }

        private void StylizePanel(VisualElement panel, Firearms weapon)
        {
            Color color = WeaponsPanelControl.Instance.WeaponRarityToColor[weapon.Rarity];

            panel.style.borderBottomColor = color;
            panel.style.borderLeftColor = color;
            panel.style.borderRightColor = color;


            VisualElement title = panel.Q<VisualElement>("Title");
            VisualElement icon = panel.Q<VisualElement>("Content").Q<VisualElement>("Icon");
            VisualElement specific = panel.Q<VisualElement>("Content").Q<VisualElement>("Stats").Q<VisualElement>("Specific");
            VisualElement bonus = panel.Q<VisualElement>("Content").Q<VisualElement>("Stats").Q<VisualElement>("Bonus");


            title.Q<Label>("Name").text = weapon.WeaponName;
            title.Q<Label>("Name").style.color = color;
            title.Q<Label>("Type").text = weapon.Type.ToString();

            icon.style.backgroundImage = new StyleBackground(weapon.IconImage);
            icon.style.unityBackgroundImageTintColor = color;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

            specific.Q<VisualElement>("Rarity").Q<Label>("Data").text = weapon.Rarity.ToString();
            specific.Q<VisualElement>("Rarity").Q<Label>("Data").style.color = color;

            specific.Q<VisualElement>("Damage").Q<Label>("Data").text = weapon.Damage.ToString();

            specific.Q<VisualElement>("Element").Q<Label>("Data").text = weapon.Element.ToString();
            specific.Q<VisualElement>("Element").Q<Label>("Data").style.color = Element.Instance.TypeToColor[weapon.Element];

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

        private void StylizePanel(VisualElement panel, Attachment attachment)
        {
            Color color = WeaponsPanelControl.Instance.AttachmentRarityToColor[attachment.Rarity];

            panel.style.borderBottomColor = color;
            panel.style.borderLeftColor = color;
            panel.style.borderRightColor = color;


            VisualElement title = panel.Q<VisualElement>("Title");
            VisualElement icon = panel.Q<VisualElement>("Content").Q<VisualElement>("Icon");
            VisualElement specific = panel.Q<VisualElement>("Content").Q<VisualElement>("Stats").Q<VisualElement>("Specific");
            VisualElement bonus = panel.Q<VisualElement>("Content").Q<VisualElement>("Stats").Q<VisualElement>("Bonus");


            title.Q<Label>("Name").text = attachment.AttachmentName;
            title.Q<Label>("Name").style.color = color;
            title.Q<Label>("Type").text = attachment.Type.ToString();

            icon.style.backgroundImage = new StyleBackground(attachment.IconImage);
            icon.style.unityBackgroundImageTintColor = color;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

            specific.Q<VisualElement>("Rarity").Q<Label>("Data").text = attachment.Rarity.ToString();
            specific.Q<VisualElement>("Rarity").Q<Label>("Data").style.color = color;

            var list = attachment.Bonus.GetBonusDescriptionList();
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
