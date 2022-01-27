using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private WeaponsView weaponsView;

        [SerializeField] private int maxWeaponNum = 5;
        [SerializeField] private List<Weapon> weaponList;
        // mod list

        // test 
        [SerializeField] private Weapon weaponForTest;

        private void Start()
        {
            AddWeapon(weaponForTest);
        }

        public void AddWeapon(Weapon weapon)
        {
            if (weaponList.Count < maxWeaponNum)
            {
                weaponList.Add(weapon);
                UpdateAll();
            }
        }

        private void UpdateAll()
        {
            weaponsView.UpdateWeaponList(weaponList);
        }
    }
}
