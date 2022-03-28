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

        [TextArea(5, 10)]
        [SerializeField] private string citation;

        private Attachment attachment;
        private AttachmentBonus bonus;

        public void Setup(Attachment.AttachmentRarity rarity)
        {
            CurrentItemType = ItemType.Attachment;

            GameObject obj = new GameObject();
            attachment = obj.AddComponent<Attachment>();

            bonus = new AttachmentBonus(attachment);
            attachment.SetSkill = PlayerSkillManager.Instance.GetSetSkill(set);

            //attachment.Set = set;
            //attachment.Type = type;
            //attachment.Rarity = rarity;
            //attachment.IconImage = iconImage;
            //attachment.Bonus = bonus;


            attachment.gameObject.name = attachment.AttachmentName;

            attachment.Initialize(Type, rarity, set, bonus, iconImage, citation);

            SetupVfx(WeaponsPanelControl.Instance.AttachmentRarityToColor[rarity]);
        }

        public Attachment.AttachmentSet Set { get { return set; } set { set = value; } }
        public Attachment.AttachmentType Type { get { return type; } }
        public Attachment Attachment { get { return attachment; } set { attachment = value; } }
        public AttachmentBonus Bonus { get { return bonus; } set { bonus = value; } }
    }
}
