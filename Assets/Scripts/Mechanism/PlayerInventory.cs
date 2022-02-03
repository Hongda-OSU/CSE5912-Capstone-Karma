using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private WeaponsPanelControl weaponsPanelControl;

        [SerializeField] private int maxWeaponNum = 5;
        [SerializeField] private Weapon[] weapons;
        // mod list

        // test 
        [SerializeField] private Weapon[] weaponsForTest;

        private void Awake()
        {
            weapons = new Weapon[maxWeaponNum];
        }
        private void Start()
        {
            foreach (Weapon weapon in weaponsForTest)
                AddWeapon(weapon);
        }

        public void AddWeapon(Weapon weapon)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] == null)
                {
                    weapons[i] = weapon;
                    break;
                }
            }
            UpdateAll();
        }

        private void UpdateAll()
        {
            weaponsPanelControl.UpdateWeaponList(weapons);
        }
    }
}
