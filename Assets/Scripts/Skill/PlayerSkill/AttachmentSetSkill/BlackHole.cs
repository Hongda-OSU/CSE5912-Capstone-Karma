using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class BlackHole : MonoBehaviour
    {
        [SerializeField] private float pullSpeed;
        [SerializeField] private float radius;

        public void Initialize(float speed, float r)
        {
            pullSpeed = speed;
            radius = r;
        }

        private void FixedUpdate()
        {

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

            foreach (var hitCollider in hitColliders)
            {
                hitCollider.TryGetComponent(out Enemy enemy);
                if (enemy == null)
                    continue;

                Vector3 direction = (transform.position - enemy.transform.position).normalized;
                enemy.Agent.Move(direction * pullSpeed * Time.deltaTime);
            }
        }
    }
}
