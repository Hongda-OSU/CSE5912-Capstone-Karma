using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Damager_collision : MonoBehaviour
    {
        [SerializeField] private float baseDamage;
        [SerializeField] private Element.Type type;
        private Enemy source;
        private bool isPlayerHit = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            var target = PlayerStats.Instance;
            if (target == null)
                return;

            Damage damage = new Damage(baseDamage, type, source, target);
            
            target.TakeDamage(damage);

            isPlayerHit = true;
        }

        public Enemy Source { get { return source; } set { source = value; } }
        public float BaseDamage { get { return baseDamage; } set { baseDamage = value; } }
        public Element.Type Type { get { return type; } set { type = value; } }
        public bool IsPlayerHit { get { return isPlayerHit; } }
    }
}
