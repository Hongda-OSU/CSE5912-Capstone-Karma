using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Attachment : MonoBehaviour
    {
        [SerializeField] private string attachmentName;
        [SerializeField] private AttachmentType attachmentType;
        [SerializeField] private AttachmentRarity rarity = AttachmentRarity.Common;
        [SerializeField] private Firearms attachedTo;
        private AttachmentBonus attachmentBonus;

        // UI related
        [Header("UI related")]
        private Sprite iconImage;

        public enum AttachmentType
        {
            Bullet,
            Scope,
            Casing,
        }
        public enum AttachmentRarity
        {
            Common = 0,
            Rare = 1,
            Epic = 2,
            Legendary = 3,
            Divine = 4,
        }


        //protected virtual void Update()
        //{
        //    attachmentBonus.Perform(true);
        //}
        //private void OnDisable()
        //{
        //    attachmentBonus.Perform(false);
        //}


        public string AttachmentName { get { return attachmentName; } set { attachmentName = value; } }
        public AttachmentType Type { get { return attachmentType; } }
        public AttachmentRarity Rarity { get { return rarity; } set { rarity = value; } }
        public Firearms AttachedTo { get { return attachedTo; } set { attachedTo = value; } }
        public AttachmentBonus Bonus { get { return attachmentBonus; } set { attachmentBonus = value; } }
        public Sprite IconImage { get { return iconImage; } set { iconImage = value; } }
    }
}
