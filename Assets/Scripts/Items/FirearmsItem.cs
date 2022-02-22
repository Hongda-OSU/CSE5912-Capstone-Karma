using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FirearmsItem : BaseItem
    {
        public enum FirearmsType { AssaultRifle, HandGun, Other }
        public FirearmsType CurrentFirearmsType;
        public string ArmsName;

        public void HideItem()
        {
            this.gameObject.SetActive(false);
        }
    }
}