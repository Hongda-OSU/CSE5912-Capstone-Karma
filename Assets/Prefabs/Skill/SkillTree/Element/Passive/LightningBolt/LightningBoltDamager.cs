using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LightningBoltDamager : MonoBehaviour
    {
        private LightningBolt owner;

        private float speed = 0.1f;
        private Vector3 prevPosition;

        private void Awake()
        {
            StartCoroutine(Boost());
        }
        private void Update()
        {

            prevPosition = transform.position;

            transform.Translate(0, 0, speed * Time.deltaTime);

            if (!Physics.Raycast(prevPosition,
                (transform.position - prevPosition).normalized,
                out RaycastHit hit,
                (transform.position - prevPosition).magnitude)) return;

            hit.transform.TryGetComponent(out Enemy enemy);
            if (enemy == null)
            {
                Destroy(gameObject, 5f);
            }
            else
            {
                owner.PerformDamage(enemy);
                Destroy(gameObject);
            }
        }

        private IEnumerator Boost()
        {
            yield return new WaitForSeconds(0.5f);
            speed = Owner.Speed;
        }
        public LightningBolt Owner { get { return owner; } set { owner = value; } }
    }
}
