using UnityEngine;
    public class Attachment : BaseItem
    {
        public enum AttachmentType { 
            Scope, 
            Other
        };

        public ItemType CurrentItemType;
        public int ItemId;
        public string ItemName;
    }

