using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Sergeant76 : PlayerSkill
    {
        [Header("Sergeant 76")]
        [SerializeField] private float speed;
        [SerializeField] private float acceleration;
        [SerializeField] private float distance;


        protected override string GetBuiltSpecific()
        {
            return "";
        }

        private void Update()
        {
            if (!isLearned || !isReady)
                return;

            if (WeaponManager.Instance.CarriedWeapon.wasBulletFiredThisFrame)
            {
                var bullet = WeaponManager.Instance.CarriedWeapon.bulletFired;

                StartCoroutine(Perform(bullet, LockTarget()));
            }
        }
        private GameObject LockTarget()
        { 
            var enemyList = EnemyManager.Instance.GetEnemiesInView(distance);

            float min = -1f;
            GameObject target = null;
            foreach (var go in enemyList)
            {
                var enemy = go.GetComponent<Enemy>();
                if (!enemy.IsAlive)
                    continue;

                float distance = Vector3.Distance(go.transform.position, PlayerManager.Instance.Player.transform.position);
                if (distance < min || min < 0f)
                {
                    min = distance;
                    target = go;
                }
            }
            return target;
        }
        private IEnumerator Perform(Bullet bullet, GameObject target)
        {
            bullet.BulletSpeed = speed;

            while (bullet != null && target != null)
            {
                var position = target.transform.position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;

                bullet.Direction += (position - bullet.transform.position) * acceleration * Time.deltaTime;
                bullet.Direction.Normalize();

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

    }
}
