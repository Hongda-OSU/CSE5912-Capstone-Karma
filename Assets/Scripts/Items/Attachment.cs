using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Attachment : BaseItem
    {
        public string attachmentName;
        public Sprite iconImage;
        public string description;
        public Firearms attachedTo;

        public enum AttachmentType { Scope, Other }

        public AttachmentType CurrentAttachmentType;

        public string BuildDescription()
        {
            description =
                "Name: " + attachmentName +
                "\nAttached to: " + attachedTo;

            return description;
        }
    }
}
