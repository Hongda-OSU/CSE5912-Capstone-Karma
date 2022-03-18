using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Attachment : MonoBehaviour
    {
        [SerializeField] private Firearms attachedTo;

        [SerializeField] private string attachmentName;
        [SerializeField] private AttachmentType attachmentType;
        [SerializeField] private AttachmentRarity rarity = AttachmentRarity.Common;
        [SerializeField] private AttachmentSet attachmentSet;

        private AttachmentBonus attachmentBonus;

        // UI related
        [Header("UI related")]
        private Sprite iconImage;

        // only divine trigger set effect
        public enum AttachmentSet
        {
            QuantumBreak,
            B,
            C,
        }
        public enum AttachmentType
        {
            Bullet = 0,
            Scope = 1,
            Casing = 2,
            Rune = 3,
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
            PerformBonus(weapon == WeaponManager.Instance.CarriedWeapon);
            attachedTo = weapon;
        }

        public void PerformBonus(bool enabled)
        {
            attachmentBonus.Perform(enabled);
        }


        public string AttachmentName { get { return attachmentName; } set { attachmentName = value; } }
        public AttachmentSet Set { get { return attachmentSet; } set { attachmentSet = value; } }
        public AttachmentType Type { get { return attachmentType; } set { attachmentType = value; } }
        public AttachmentRarity Rarity { get { return rarity; } set { rarity = value; } }
        public Firearms AttachedTo { get { return attachedTo; } }
        public AttachmentBonus Bonus { get { return attachmentBonus; } set { attachmentBonus = value; } }
        public Sprite IconImage { get { return iconImage; } set { iconImage = value; } }


    }

}
