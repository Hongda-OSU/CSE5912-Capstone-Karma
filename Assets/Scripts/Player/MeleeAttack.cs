using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class MeleeAttack : PlayerSkill
    {
        [Header("Melee Attack")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private Element.Type elementType = Element.Type.Physical;
        [SerializeField] private float extraSpeed = 0f;
        [SerializeField] private float range = 1f;
        [SerializeField] private bool isEnabled = true;


        private static MeleeAttack instance;
        public static MeleeAttack Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;
        }

        public void Perform()
        {
            if (!isEnabled)
                return;

            var position = transform.position + WeaponManager.Instance.GetShootDirection() * range / 2;
            var size = new Vector3(range, range, range);

            Collider[] hitColliders = Physics.OverlapBox(position, size);

            foreach (var hitCollider in hitColliders)
            {
                hitCollider.TryGetComponent(out Enemy enemy);
                if (enemy == null || !enemy.IsAlive)
                    continue;

                var damage = new Damage(baseDamage, elementType, PlayerStats.Instance, enemy);
                PlayerManager.Instance.PerformSkillDamage(enemy, damage);
            }
        }


        public void IncreaseDamage(float damage)
        {
            baseDamage += damage;
        }
        public void IncreaseAttackSpeed(float speed)
        {
            var animator = WeaponManager.Instance.CarriedWeapon.GunAnimator;

            var current = animator.GetFloat("ReloadSpeed");
            animator.SetFloat("ReloadSpeed", current + speed);
        }
        //public void IncreaseAttackRange(float range)
        //{
        //    sphereCollider.radius += range;
        //}


        public bool IsEnabled { get { return isEnabled; } set { isEnabled = value; } }
        public Element.Type ElementType { get { return elementType; } }
    }
}
