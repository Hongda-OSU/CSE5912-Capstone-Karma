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
            Bullet = 0,
            Scope = 1,
            Casing = 2,
        }
        public enum AttachmentRarity
        {
            Common = 0,
            Rare = 1,
            Epic = 2,
            Legendary = 3,
            Divine = 4,
        }

        public void AttachTo(Firearms weapon)
        {
            if (weapon == WeaponManager.Instance.CarriedWeapon)
            {
                PerformBonus(true);
            }
            else
            {
                PerformBonus(false);
            }
            attachedTo = weapon;
        }

        public void PerformBonus(bool enabled)
        {
            attachmentBonus.Perform(enabled);
        }


        public string AttachmentName { get { return attachmentName; } set { attachmentName = value; } }
        public AttachmentType Type { get { return attachmentType; } set { attachmentType = value; } }
        public AttachmentRarity Rarity { get { return rarity; } set { rarity = value; } }
        public Firearms AttachedTo { get { return attachedTo; } }
        public AttachmentBonus Bonus { get { return attachmentBonus; } set { attachmentBonus = value; } }
        public Sprite IconImage { get { return iconImage; } set { iconImage = value; } }
    }
}
