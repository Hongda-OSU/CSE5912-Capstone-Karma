using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{

    [RequireComponent(typeof(Collider))]
    public class Damager_collision : MonoBehaviour
    {
        [SerializeField] private float baseDamage;
        [SerializeField] private Element.Type type;
        [SerializeField] private bool hitBack = false;
        [SerializeField] private LayerMask layerMask;

        private Enemy enemy;
        [SerializeField] private bool isPlayerHit = false;
        [SerializeField] private GameObject objectHit;

        private Collider collider3d;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            collider3d = GetComponent<Collider>();
        }

        private void Update()
        {
            Collider[] hitColliders = new Collider[0];
            if (collider3d is SphereCollider sphere)
                hitColliders = Physics.OverlapSphere(transform.position, sphere.radius, layerMask);
            else if (collider3d is BoxCollider box)
                hitColliders = Physics.OverlapBox(transform.position, box.size / 2, transform.rotation, layerMask);

            if (hitColliders.Length == 0)
                return;

            objectHit = hitColliders[0].gameObject;
        }

        private void OnTriggerStay(Collider other)
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
        public bool IsPlayerHit { get { return isPlayerHit; } set { isPlayerHit = value; } }
        public GameObject Hit { get { return objectHit; } set { objectHit = value; } }
    }
}
