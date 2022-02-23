using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class AttachmentItem : BaseItem
    {
        [SerializeField] private Attachment.AttachmentType type;
        private Attachment weapon;
        private AttachmentBonus bonus;


        public void AssignWeapon(Attachment attachment)
        {
            this.weapon = attachment;
        }


        public Attachment.AttachmentType Type { get { return type; } }
        public Attachment Weapon { get { return weapon; } set { weapon = value; } }
        public AttachmentBonus Bonus { get { return bonus; } set { bonus = value; } }
    }
}
