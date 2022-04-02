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
        private PlayerSkill setSkill;
        [SerializeField] private string attachmentRealName;

        // UI related
        [Header("UI related")]
        [SerializeField] private Sprite iconImage;

        private string citation;

        // only divine trigger set effect
        public enum AttachmentSet
        {
            AtomBreak,
            VoidKnight,
            Sergeant76,
            BlackSoul,
            Leviathan,
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

        public void Initialize(AttachmentType type, AttachmentRarity rarity, AttachmentSet set, Sprite image, string citation, int runeIndex)
        {
            attachmentName = rarity.ToString() + " " + type.ToString();

            attachmentType = type;
            this.rarity = rarity;
            attachmentSet = set;
            iconImage = image;
            this.citation = citation;

            attachmentBonus = new AttachmentBonus(this);
            if (runeIndex > -1)
            {
                attachmentBonus.GetRuneBonus(runeIndex);
                this.rarity = AttachmentRarity.Divine;
            }
            setSkill = PlayerSkillManager.Instance.GetSetSkill(attachmentSet);
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
        public PlayerSkill SetSkill { get { return setSkill; } set { setSkill = value; } }
        public Sprite IconImage { get { return iconImage; } set { iconImage = value; } }
        public string Citation { get { return citation; } set { citation = value; } }
        public string AttachmentRealName { get { return attachmentRealName; } set { attachmentRealName = value; } }
    }

}
