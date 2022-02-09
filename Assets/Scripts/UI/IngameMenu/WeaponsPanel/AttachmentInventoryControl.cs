using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class AttachmentInventoryControl : UI
    {
        public List<AttachmentInventorySlot> slotList;

        public VisualElement attachmentsInventory;

        [SerializeField] private PageIndicatorControl pageIndicatorControl;

        private List<Attachment> attachmentList;

        private int currentPage = 0;
        private int totalPages = 0;
        public int TotalPages { get { return totalPages; } }

        private void Awake()
        {
            Initialize();

            attachmentsInventory = root.Q<VisualElement>("AttachmentInventory");

            attachmentsInventory.style.display = DisplayStyle.None;

            slotList = new List<AttachmentInventorySlot>();

            for (int i = 0; i < attachmentsInventory.Q<VisualElement>("AttachmentInventorySlots").childCount; i++)
            {
                VisualElement row = attachmentsInventory.Q<VisualElement>("AttachmentInventoryRow_" + i);
                for (int j = 0; j < row.childCount; j++)
                {
                    VisualElement slot = row.Q<VisualElement>("AttachmentInventorySlot_" + j);
                    slotList.Add(new AttachmentInventorySlot(slot));
                }
            }
        }

        public bool IsInventoryOpened()
        {
            if (attachmentsInventory.style.display == DisplayStyle.Flex)
            {
                return true;
            }
            return false;
        }
        public Attachment FindAttachmentBySlot(VisualElement target)
        {
            Attachment attachment = null;

            foreach (var item in slotList)
            {
                if (item.slot == target)
                    attachment = item.attachment;
            }

            return attachment;
        }

        public void UpdateAttachmentInventory(List<Attachment> attachmentList)
        {
            this.attachmentList = attachmentList;

            totalPages = attachmentList.Count / slotList.Count;
            pageIndicatorControl.SetIndicatorNum(totalPages);

            ClearAttachmentInventory();

            if (attachmentList.Count > 0)
            {
                for (int i = 0; i < attachmentList.Count; i++)
                {
                    if (i >= slotList.Count)
                        return;

                    Attachment attachment = attachmentList[i];
                    VisualElement slot = slotList[i].slot;

                    slotList[i].attachment = attachment;

                    slot.Q<VisualElement>("AttachmentIcon").style.backgroundImage = new StyleBackground(attachment.iconImage);
                    slot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                }
            }
        }


        public void FlipInventoryPage(int steps)
        {
            if (attachmentsInventory.style.display != DisplayStyle.Flex)
                return;

            int numPerPage = slotList.Count;
            int numOfAttachments = attachmentList.Count;
            int numOfPages = numOfAttachments / numPerPage + 1;

            if (numOfAttachments % numPerPage == 0)
                return;

            currentPage = Mathf.Clamp(currentPage + steps, 0, numOfPages - 1);

            int numOnPage = Mathf.Clamp(numOfAttachments - numPerPage * currentPage, 0, numPerPage);

            ClearAttachmentInventory();

            for (int i = 0; i < numOnPage; i++)
            {
                int index = i + numOnPage * currentPage;
                Attachment attachment = attachmentList[index];
                VisualElement slot = slotList[i].slot;

                slotList[i].attachment = attachment;

                slot.Q<VisualElement>("AttachmentIcon").style.backgroundImage = new StyleBackground(attachment.iconImage);
                slot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            }
        }

        private void ClearAttachmentInventory()
        {
            foreach (var inventorySlot in slotList)
            {
                inventorySlot.attachment = null;
                inventorySlot.attachmentIcon.style.backgroundImage = null;
            }
        }
    }
}