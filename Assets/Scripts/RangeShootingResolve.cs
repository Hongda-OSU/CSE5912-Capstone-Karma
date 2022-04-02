using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class RangeShootingResolve : MonoBehaviour
    {
        public ParticleSystem part;
        private float damageAmount;
        private float impactAmount;
        private IDamageable sourceFrom;
        
        void Start()
        {
            part = GetComponent<ParticleSystem>();
        } 

        void OnParticleCollision(GameObject other)
        {
            float damageCalculation;
            if (other.tag == "Player")
            {
                damageCalculation = damageAmount + Mathf.RoundToInt(Random.Range(-2f, 4f));
                Damage damage = new Damage(Mathf.RoundToInt(Random.Range(8f, 10f)), Element.Type.Physical, sourceFrom, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), impactAmount);
            }
        }

        public void SetVariables(float damage, float impact, IDamageable source)
        {
            damageAmount = damage;
            impactAmount = impact;
            sourceFrom = source;
        }

        //public float Damage { set { damage = value; } }
        //public float Impact { set { impact = value; } }
        //public IDamageable Source { set { source = value; } }
    }

}

