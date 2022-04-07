using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class MageAttack : MonoBehaviour
    {
        public float Speed;
        public float Damage;
        public float Force;
        private IDamageable source;
        private Transform bulletTransform;
        private Vector3 Direction;
        private Vector3 prevPosition;
        public Vector3 hitPosition;
        public GameObject ImpactPrefab;
        public GameObject targetHit;
     

        void Start()
        {
            bulletTransform = transform;
            prevPosition = bulletTransform.position;
            Direction = bulletTransform.forward;
        }
        void Update()
        {
            prevPosition = bulletTransform.position;
            Direction.Normalize();
            transform.position += Direction * Speed * Time.deltaTime;
            // Check hit anything
            if (!Physics.Raycast(prevPosition,
                (bulletTransform.position - prevPosition).normalized,
                out RaycastHit hit,
                (bulletTransform.position - prevPosition).magnitude)) return;
            CheckTargetHit(hit);
        }

        public void SetVariables(float speed, float damage, float force, GameObject impact, IDamageable sourceFrom)
        {
            Speed = speed;
            Damage = damage;
            Force = force;
            ImpactPrefab = impact;
            source = sourceFrom;
        }

        private void CheckTargetHit(RaycastHit hit)
        {
            hitPosition = hit.point;
            targetHit = hit.transform.gameObject;
            GameObject vfx = Instantiate(ImpactPrefab, hitPosition, Quaternion.identity);
            Destroy(vfx, 1f);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Damage damage = new Damage(Damage + Mathf.RoundToInt(Random.Range(-2f, 4f)), Element.Type.Fire, source, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), Force);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
