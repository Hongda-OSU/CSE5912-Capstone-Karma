using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CSE5912.PolyGamers
{
    public class WeaponSkinManager : MonoBehaviour
    {
        [SerializeField] private List<Material> weaponSkins;

        private static WeaponSkinManager instance;
        public static WeaponSkinManager Instance { get { return instance; } }
        private int counter = 0;
        private int count;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            count = weaponSkins.Count;
        }

        public void ChangeCurrentWeaponSkin()
        {
            //weaponSkins[counter].mainTextureScale = new Vector2(Random.Range(40f, 80f), Random.Range(40f, 80f));
            //WeaponManager.Instance.CarriedWeapon.transform.Find("arms/WeaponSkin").GetComponent<Renderer>().material = weaponSkins[counter];
            counter++;
            if (counter >= count)
                counter = 0;
        }
    }
}
