using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FlameCollider : MonoBehaviour
    {
        private float damageAmount;
        private float force;
        private IDamageable source;

        void Update()
        {
            StartCoroutine(DestorySelf());
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Damage damage = new Damage(damageAmount / 5f + Mathf.RoundToInt(Random.Range(-2f, 4f)), Element.Type.Fire, source, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(-FPSControllerCC.Instance.transform.forward, force);
                    FPSControllerCC.Instance.AddImpact(Vector3.up, force / 1.5f);
                }
            }
        }

        public void SetVariables(float damage, float impact, IDamageable sourceFrom)
        {
            damageAmount = damage;
            force = impact;
            source = sourceFrom;
        }

        private IEnumerator DestorySelf()
        {
            yield return new WaitForSeconds(5f);
            if (this.gameObject != null)
                Destroy(this.gameObject, 1f);
        }

    }
}
