using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class BaseItem : MonoBehaviour
    {
        public enum ItemType { Firearms, Attachment, Others }
        public ItemType CurrentItemType;
        public string ItemName;
        public int ItemId;
    }
}