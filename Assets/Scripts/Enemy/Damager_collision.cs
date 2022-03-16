using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{

    [RequireComponent(typeof(SphereCollider))]
    public class Damager_collision : MonoBehaviour
    {
        [SerializeField] private float baseDamage;
        [SerializeField] private Element.Type type;
        [SerializeField] private bool hitBack = false;
        [SerializeField] private LayerMask layerMask;

        private Enemy enemy;
        private bool isPlayerHit = false;
        private GameObject objectHit;

        private SphereCollider collider3d;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            collider3d = GetComponent<SphereCollider>();
        }

        private void Update()
        {

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, collider3d.radius, layerMask);

            if (hitColliders.Length == 0)
                return;

            objectHit = hitColliders[0].gameObject;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player") || isPlayerHit)
                return;

            var target = PlayerStats.Instance;
            if (target == null)
                return;

            if (enemy == null)
            {
                return;
            }
            Damage damage = new Damage(baseDamage, type, enemy, target);
            
            target.TakeDamage(damage);
            if (hitBack)
                target.HitBack((target.transform.position - transform.position).normalized, damage.ResolvedValue);

            isPlayerHit = true;
        }

        public void Initialize(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public Enemy Enemy { get { return enemy; } }
        public float BaseDamage { get { return baseDamage; } set { baseDamage = value; } }
        public Element.Type Type { get { return type; } set { type = value; } }
        public bool IsPlayerHit { get { return isPlayerHit; } }
        public GameObject Hit { get { return objectHit; } }
    }
}
