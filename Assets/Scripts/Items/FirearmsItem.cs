using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FirearmsItem : BaseItem
    {
        [SerializeField] private Firearms.WeaponType type;
        private Firearms weapon;
        private WeaponBonus bonus;

        private float rotateSpeed = 90f;

        private float floatSpeed = 1f;
        private float floatHeight = 0.1f;

        public void AssignWeapon(Firearms weapon)
        {
            this.weapon = weapon;
        }

        private void Update()
        {
            transform.RotateAround(transform.position, transform.up, rotateSpeed * Time.deltaTime);

            //Vector3 pos = transform.position;
            //float newY = Mathf.Sin(Time.time * floatSpeed);
            //transform.position = new Vector3(pos.x, newY, pos.z) * floatHeight;
            //Debug.Log(transform.position);
        }

        public Firearms.WeaponType Type { get { return type; } }
        public Firearms Weapon { get { return weapon; } set { weapon = value; } }
        public WeaponBonus Bonus { get { return bonus; } set { bonus = value; } }
    }
}