using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class Bullet : MonoBehaviour
    {
        public float BulletSpeed;
        private Rigidbody bulletRigidBody;
        private Transform bulletTransform;

        void Start()
        {
            bulletRigidBody = GetComponent<Rigidbody>();
            bulletTransform = transform;
        }

        private void FixedUpdate()
        {
            bulletRigidBody.velocity = bulletTransform.forward * BulletSpeed * Time.fixedTime;
        }
    }
}
