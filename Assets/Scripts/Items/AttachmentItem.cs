using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class AttachmentItem : BaseItem
    {
        [SerializeField] private Sprite iconImage;
        [SerializeField] private Attachment.AttachmentType type;

        [TextArea(5, 10)]
        [SerializeField] private string citation;

        private Attachment attachment;

        public void Setup(Attachment.AttachmentRarity rarity)
        {
            CurrentItemType = ItemType.Attachment;
           
            GameObject obj = new GameObject();
            attachment = obj.AddComponent<Attachment>();

            // set attachment real name
            attachment.AttachmentRealName = this.gameObject.name.Replace("(Clone)", "");


            //attachment.Set = set;
            //attachment.Type = type;
            //attachment.Rarity = rarity;
            //attachment.IconImage = iconImage;
            //attachment.Bonus = bonus;


            attachment.gameObject.name = attachment.AttachmentName;

            attachment.Initialize(type, rarity, iconImage, citation);

            SetupVfx(WeaponsPanelControl.Instance.AttachmentRarityToColor[rarity]);
        }

        public Attachment.AttachmentType Type { get { return type; } }
        public Attachment Attachment { get { return attachment; } set { attachment = value; } }
    }
}
