using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class AttachmentItem : BaseItem
    {
        [SerializeField] private Sprite iconImage;
        [SerializeField] private Attachment.AttachmentType type;
        [SerializeField] private Attachment.AttachmentSet set;

        private Attachment attachment;
        private AttachmentBonus bonus;

        public void Setup(Attachment.AttachmentRarity rarity)
        {
            CurrentItemType = ItemType.Attachment;

            GameObject obj = new GameObject();
            attachment = obj.AddComponent<Attachment>();

            attachment.Set = set;
            attachment.Type = type;
            attachment.Rarity = rarity;
            attachment.IconImage = iconImage;

            attachment.Rarity = rarity;
            bonus = new AttachmentBonus(attachment);
            attachment.Bonus = bonus;
            attachment.gameObject.name = attachment.AttachmentName;

            SetupVfx(WeaponsPanelControl.Instance.AttachmentRarityToColor[rarity]);
        }

        public Attachment.AttachmentSet Set { get { return set; } set { set = value; } }
        public Attachment.AttachmentType Type { get { return type; } }
        public Attachment Attachment { get { return attachment; } set { attachment = value; } }
        public AttachmentBonus Bonus { get { return bonus; } set { bonus = value; } }
    }
}
