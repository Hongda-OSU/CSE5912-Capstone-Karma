using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CSE5912.PolyGamers
{
    public class MeleeAttack : PlayerSkill
    {
        [Header("Melee Attack")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private Element.Type elementType = Element.Type.Physical;
        //[SerializeField] private float extraSpeed = 0f;
        [SerializeField] private float range = 1f;
        [SerializeField] private bool isEnabled = true;

        [SerializeField] private AudioSource sfx;
        [SerializeField] private AudioClip[] sounds;

        public UnityEvent meleeEvent = new UnityEvent();

        private static MeleeAttack instance;
        public static MeleeAttack Instance { get { return instance; } }

        protected override string GetBuiltSpecific()
        {
            return "";
        }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;
        }

        public void Perform()
        {
            if (!isReady)
                return;

            StartCoroutine(CoolDown());

            sfx.clip = sounds[Random.Range(0, sounds.Length)];
            sfx.Play();

            FPSControllerCC.Instance.animator.SetTrigger("KnifeAttack");
            meleeEvent.Invoke();
        }

        public void PerformDamage()
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

                var damage = new Damage(baseDamage * PlayerStats.Instance.MeleeDamageFactor, elementType, PlayerStats.Instance, enemy);
                PlayerManager.Instance.PerformSkillDamage(enemy, damage);
            }
        }

        private IEnumerator CoolDown()
        {
            isReady = false;

            timeSincePerformed = 0f;
            while (timeSincePerformed < cooldown / PlayerStats.Instance.MeleeSpeedFactor)
            {
                timeSincePerformed += Time.unscaledDeltaTime;
                yield return new WaitForSeconds(Time.unscaledDeltaTime);
            }

            isReady = true;
        }


        //public void IncreaseAttackRange(float range)
        //{
        //    sphereCollider.radius += range;
        //}


        public bool IsEnabled { get { return isEnabled; } set { isEnabled = value; } }
        public Element.Type ElementType { get { return elementType; } set { elementType = value; } }
        public float BaseDamage { get { return baseDamage; } set { baseDamage = value; } }
    }
}
