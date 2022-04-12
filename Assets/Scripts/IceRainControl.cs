using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class IceRainControl : MonoBehaviour
    {
        private float Speed;
        private float Damage;
        private float Force;
        private Element.Type Type;
        private IDamageable Source;
        private Transform bulletTransform;
        private Vector3 Direction;
        private GameObject impactPrefab;
        private float yPos;

        void Start()
        {
            bulletTransform = transform;
            Direction = bulletTransform.forward;
        }

        void Update()
        {
            Direction.Normalize();
            transform.position += Direction * Speed * Time.deltaTime;
        }

        public void SetVariables(float speed, float damage, float force, IDamageable sourceFrom, Element.Type type, GameObject impact, float y)
        {
            Speed = speed;
            Damage = damage;
            Force = force;
            Type = type;
            Source = sourceFrom;
            impactPrefab = impact;
            yPos = y;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Damage damage = new Damage(Damage / 5f + Mathf.RoundToInt(Random.Range(-2f, 4f)), Type, Source, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(-FPSControllerCC.Instance.transform.forward, Force);
                    Vector3 pos = other.transform.position;
                    GameObject vfx = Instantiate(impactPrefab, new Vector3(pos.x, yPos, pos.z), Quaternion.identity);
                    Destroy(vfx, 1f);
                    Destroy(this.gameObject);
                }
            }
        }

    }
}
