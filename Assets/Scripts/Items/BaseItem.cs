using UnityEngine;

    public abstract class BaseItem : MonoBehaviour
    {
        public enum ItemType { Firearms, Others}
        public ItemType CurrentItemType;
        public int ItemId;


    }
