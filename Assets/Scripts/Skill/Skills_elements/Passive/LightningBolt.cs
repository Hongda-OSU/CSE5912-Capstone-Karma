using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LightningBolt : Skill
    {

        [Header("Lightning Bolt")]
        [SerializeField] private GameObject vfxPrefab;
        [SerializeField] private Vector3 pivot;

        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float damagePerLevel = 10f;

        [SerializeField] private int baseLightningNumber = 1;
        [SerializeField] private int lightningNumberPerLevel = 1;

        [SerializeField] private float speed = 10f;
        [SerializeField] private float boltTimeGap = 0.1f;
        [SerializeField] private Vector3 offset;

        private Bullet prevBullet;

        private void Update()
        {
            var bullet = WeaponManager.Instance.CarriedWeapon.bulletFired;
            if (bullet != null && bullet != prevBullet)
            {
                StartCoroutine(Perform(bullet.transform));
            }
            prevBullet = bullet;
        }
        private IEnumerator Perform(Transform point)
        {
            if (!isReady)
                yield break;

            StartCoolingdown();

            var position = point.position +
                point.right * pivot.x + point.up * pivot.y + point.forward * pivot.z;
            var rotation = point.rotation;

            int num = baseLightningNumber + lightningNumberPerLevel * (level - 1);
            for (int i = 0; i < num; i++)
            {
                yield return new WaitForSeconds(boltTimeGap);
                StartCoroutine(Shoot(position, rotation));
            }
        }

        private IEnumerator Shoot(Vector3 position, Quaternion rotation)
        {
            var vfx = Instantiate(vfxPrefab);

            vfx.GetComponent<LightningBoltDamager>().Owner = this;

            var randOffset = new Vector3(Random.Range(-offset.x, offset.x), Random.Range(-offset.y, offset.y), Random.Range(-offset.z, offset.z));
            vfx.transform.position = position + randOffset;
            vfx.transform.rotation = rotation;

            yield return null;
        }


        public void PerformDamage(Enemy enemy)
        {
            float value = baseDamage + damagePerLevel * (level - 1);
            Damage damage = new Damage(value, Element.Type.Electro, PlayerStats.Instance, enemy);
            PlayerManager.Instance.PerformSkillDamage(enemy, damage);
        }

        public float Speed { get { return speed; } }
    }
}
