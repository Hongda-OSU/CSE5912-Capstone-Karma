using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class AttachmentInventorySlot
    {
        public VisualElement slot;
        public Attachment attachment;
        public VisualElement attachmentIcon;

        public AttachmentInventorySlot(VisualElement inventorySlot)
        {
            slot = inventorySlot;
            attachment = null;
            attachmentIcon = inventorySlot.Q<VisualElement>("AttachmentIcon");
        }
    }
}
