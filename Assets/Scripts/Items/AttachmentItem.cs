using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class AttachmentItem : BaseItem
    {
        [SerializeField] private Attachment.AttachmentType type;
        [SerializeField] private Attachment.AttachmentRarity rarity;
        [SerializeField] private Sprite iconImage;
        private Attachment attachment;
        private AttachmentBonus bonus;

        private void Awake()
        {
            CurrentItemType = ItemType.Attachment;

            GameObject obj = new GameObject();
            attachment = obj.AddComponent<Attachment>();
            attachment.Type = type;
            attachment.Rarity = rarity;
            attachment.IconImage = iconImage;
            Setup(rarity);
        }

        public void Setup(Attachment.AttachmentRarity rarity)
        {
            attachment.Rarity = rarity;
            bonus = new AttachmentBonus(attachment);
            attachment.Bonus = bonus;
            attachment.gameObject.name = attachment.AttachmentName;

            SetupVfx(WeaponsPanelControl.Instance.AttachmentRarityToColor[rarity]);
        }

        public Attachment.AttachmentType Type { get { return type; } }
        public Attachment Attachment { get { return attachment; } set { attachment = value; } }
        public AttachmentBonus Bonus { get { return bonus; } set { bonus = value; } }
    }
}
