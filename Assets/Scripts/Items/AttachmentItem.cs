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

        [Header("-1 if not rune")]
        [SerializeField] private int runeIndex = -1;

        [TextArea(5, 10)]
        [SerializeField] private string citation;

        private Attachment attachment;

        public void Setup(Attachment.AttachmentRarity rarity)
        {
            CurrentItemType = ItemType.Attachment;
           
            GameObject obj = new GameObject();
            attachment = obj.AddComponent<Attachment>();

            // set attachment real name
            attachment.AttachmentRealName = gameObject.name.Replace("(Clone)", "");

            attachment.Initialize(type, rarity, set, iconImage, citation, runeIndex);

            attachment.gameObject.name = attachment.AttachmentName;

            SetupVfx(WeaponsPanelControl.Instance.AttachmentRarityToColor[attachment.Rarity]);
        }

        public Attachment.AttachmentType Type { get { return type; } }
        public Attachment Attachment { get { return attachment; } set { attachment = value; } }
    }
}
