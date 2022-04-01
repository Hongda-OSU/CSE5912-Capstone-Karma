using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FirearmsItem : BaseItem
    {
        [SerializeField] private Firearms.WeaponType type;
        [SerializeField] private Firearms weapon;
        private WeaponBonus bonus;


        private void Awake()
        {
            CurrentItemType = ItemType.Firearms;

        }

        public void Setup(Firearms weapon)
        {
            this.weapon = weapon;

            var color = WeaponsPanelControl.Instance.WeaponRarityToColor[weapon.Rarity];
            SetupVfx(color);
        }


        public Firearms.WeaponType Type { get { return type; } }
        public Firearms Weapon { get { return weapon; } set { weapon = value; } }
        public WeaponBonus Bonus { get { return bonus; } set { bonus = value; } }
    }
}