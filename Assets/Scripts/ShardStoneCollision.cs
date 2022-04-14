using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ShardStoneCollision : MonoBehaviour
    {
        private float damageAmount;
        private float forceUp;
        private float forceBack;
        private IDamageable source;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Damage damage = new Damage(damageAmount / 5f + Mathf.RoundToInt(Random.Range(-2f, 4f)), Element.Type.Physical, source, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), forceBack);
                    FPSControllerCC.Instance.AddImpact(Vector3.up, forceUp);
                }
            }
        }

        public void SetVariables(float damage, float impactBackward, float impactUp, IDamageable sourceFrom)
        {
            damageAmount = damage;
            forceBack = impactBackward;
            forceUp = impactUp;
            source = sourceFrom;
        }
    }
}
