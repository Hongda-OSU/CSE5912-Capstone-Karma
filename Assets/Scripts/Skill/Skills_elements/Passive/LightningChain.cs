using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LightningChain : Skill
    {
        [SerializeField] private GameObject vfx;
        [SerializeField] private float displayTime = 0.3f;

        [SerializeField] private float effectDistance = 10f;

        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float damagePerLevel = 0.02f;

        private void Awake()
        {
            vfx.GetComponentInParent<LineRenderer>().enabled = false;
        }
        private void Update()
        {
            var target = PlayerManager.Instance.HitByBullet;
            if (!isReady || target == null || !target.IsAlive)
                return;

            foreach (var other in EnemyManager.Instance.EnemyList)
            {
                float distance = Vector3.Distance(target.gameObject.transform.position, other.transform.position);
                if (distance < effectDistance)
                {
                    Vector3 first = target.transform.position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;
                    Vector3 second = other.transform.position + Vector3.up * other.GetComponentInChildren<Renderer>().bounds.size.y / 2;

                    StartCoroutine(DisplayVfx(first, second));

                    Damage damage = new Damage(baseDamage, Element.Type.Electro, PlayerStats.Instance, target);
                    PlayerManager.Instance.PerformDamage(target, damage);

                    damage = new Damage(baseDamage, Element.Type.Electro, PlayerStats.Instance, other.GetComponent<Enemy>());
                    PlayerManager.Instance.PerformDamage(other.GetComponent<Enemy>(), damage);

                    PlayerManager.Instance.HitByBullet = null;
                    return;
                }

            }

        }


        private IEnumerator DisplayVfx(Vector3 first, Vector3 second)
        {
            LineRenderer lineRenderer = vfx.GetComponentInParent<LineRenderer>();

            lineRenderer.enabled = true;

            float timeSince = 0f;

            while (timeSince < displayTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                lineRenderer.SetPosition(0, first);
                lineRenderer.SetPosition(1, second);
            }

            lineRenderer.enabled = false;
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                isReady = true;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
        }
    }
}
