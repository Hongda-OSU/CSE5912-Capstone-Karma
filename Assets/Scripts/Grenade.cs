using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Grenade : MonoBehaviour
    {
        public float delay;
        public GameObject ExplosionEffect;
        private float countDown;
        private bool hasExploded;

        void Start()
        {
            countDown = delay;
        }

        void Update()
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0 && !hasExploded)
            {
                Explode();
            }
        }

        private void Explode()
        {
            hasExploded = true;
            // TODO: add explosion effect
            // Instantiate(ExplosionEffect, transform.position, transform.rotation);

            // TODO: add damage
            //Collider[] colliders = Physics.OverlapSphere(transform.position, radius: 5);
            //foreach (var collider in colliders)
            //{
                
            //}

            Destroy(gameObject);
        }
    }
}
