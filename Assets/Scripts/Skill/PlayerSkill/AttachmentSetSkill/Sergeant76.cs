using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Sergeant76 : PlayerSkill
    {
        [SerializeField] private float startSpeed;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;

        private void Update()
        {
            if (!isLearned || !isReady)
                return;

            if (WeaponManager.Instance.CarriedWeapon.wasBulletFiredThisFrame)
            {
                var bullet = WeaponManager.Instance.CarriedWeapon.bulletFired;

                StartCoroutine(Perform(bullet, LockTarget().transform));
            }
        }
        private GameObject LockTarget()
        {
            var enemyList = EnemyManager.Instance.GetEnemiesInView();

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
        private IEnumerator Perform(Bullet bullet, Transform target)
        {
            float speed = startSpeed;

            bullet.BulletSpeed = speed;

            while (bullet != null)
            {
                var position = target.position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;

                bullet.transform.LookAt(position);

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

    }
}
