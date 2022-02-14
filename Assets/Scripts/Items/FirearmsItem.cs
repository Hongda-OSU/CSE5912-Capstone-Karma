using UnityEngine;

    public class FirearmsItem : BaseItem 
    {
        public enum FirearmsType {AssaultRifle, HandGun}
        public FirearmsType CurrentFirearmsType;
        public string ArmsName;

        public void HideItem()
        {
            this.gameObject.SetActive(false);
        }
    }