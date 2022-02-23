using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Enemy Properties")]
        [SerializeField] protected string enemyName;
        [SerializeField] protected int level;
        [SerializeField] protected float experience;

        [SerializeField] protected float health;
        [SerializeField] protected float maxHealth;

        [SerializeField] protected float attackDamage;
        [SerializeField] protected float attackRange = 5f;

        protected bool isAlive = true;

        protected Debuff debuff;

        [Header("Damage")]
        [SerializeField] private float damageFactor_physical = 1f;
        [SerializeField] private float damageFactor_fire = 1f;
        [SerializeField] private float damageFactor_cryo = 1f;
        [SerializeField] private float damageFactor_electro = 1f;
        [SerializeField] private float damageFactor_venom = 1f;
        private DamageFactor damageFactor;

        [Header("Resists")]
        [SerializeField] private float physicalResist = 0f;
        [SerializeField] private float fireResist = 0f;
        [SerializeField] private float cryoResist = 0f;
        [SerializeField] private float electroResist = 0f;
        [SerializeField] private float venomResist = 0f;
        protected Resist resist;

        [Header("Random Dropoff")]
        [SerializeField] private float dropWeaponChance;
        [SerializeField] private Firearms.WeaponType dropWeaponType;
        [SerializeField] private Firearms.WeaponRarity dropWeaponRarity;

        [Header("Certain Dropoff")]
        // todo
        //[SerializeField] private

        [Header("Detection Range")]
        [SerializeField] protected float viewRadius = 15f;
        [Range(0, 360)]
        [SerializeField] protected float viewAngle = 135f;
        [SerializeField] protected float closeDetectionRange = 3f;

        protected bool foundTarget = false;
        protected bool isPlayingDeathAnimation = false;
        protected bool isAttackedByPlayer = false;

        protected float distanceToPlayer;
        protected Vector3 directionToPlayer;

        protected Transform player;
        protected Animator animator;
        protected NavMeshAgent agent;

        protected void Initialize()
        {
            player = PlayerManager.Instance.Player.transform;
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            debuff = new Debuff();

            damageFactor = new DamageFactor();
            damageFactor.SetValues(damageFactor_physical, damageFactor_fire, damageFactor_cryo, damageFactor_electro, damageFactor_venom);

            resist = new Resist();
            resist.SetValues(physicalResist, fireResist, cryoResist, electroResist, venomResist);
        }

        public virtual void TakeDamage(Damage damage)
        {
            if (!isAlive)
            {
                return;
            }

            float value = damage.ResolvedValue;

            health -= value;

            if (health <= 0)
            {
                isAlive = false;
                //test
                DropWeapon();
            }

            if (!isAttackedByPlayer)
            {
                isAttackedByPlayer = true;
            }
        }

        protected virtual void Die()
        {

        }

        public int GetDebuffStack(Debuff.DebuffType type)
        {
            return debuff.GetDebuffStack(type);
        }


        public DamageFactor GetDamageFactor()
        {
            return damageFactor;
        }
        public Resist GetResist()
        {
            return resist;
        }

        public float ComputeExtraDamage()
        {
            return 0f;
        }

        protected void DropWeapon()
        {
            if (Random.value < dropWeaponChance)
                return;

            DropoffManager.Instance.DropWeapon(dropWeaponType, dropWeaponRarity, transform.position);

        }

        protected virtual void FaceTarget(Vector3 direction)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }


        /*
         * used by animation event
         */
        protected virtual void Hit()
        {

        }
        protected virtual void FootL()
        {

        }
        protected virtual void FootR()
        {

        }

        public string EnemyName { get { return enemyName; } }
        public int Level { get { return level; } }
        public float Experience { get { return experience;} }
        public float Health { get { return health;} }
        public float MaxHealth { get { return maxHealth;} }
        public float AttackDamage { get { return attackDamage;} }
        public bool IsAttackedByPlayer { get { return isAttackedByPlayer; } }
    }
}
