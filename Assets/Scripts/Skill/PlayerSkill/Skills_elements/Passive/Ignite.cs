using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Ignite : PlayerSkill
    {

        [Header("Ignite")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float baseDamage = 50f;
        [SerializeField] private float damagePerLevel = 20f;

        [SerializeField] private float baseRadius = 1f;
        [SerializeField] private float radiusPerLevel = 0.5f;

        [SerializeField] private AudioSource sfx;

        private List<Enemy> performed;

        private void Awake()
        {
            performed = new List<Enemy>();
        }

        private void Update()
        {
            var target = PlayerManager.Instance.HitByBullet;
            if (!isLearned || target == null || target.Health > 0 || performed.Contains(target))
                return;

            Perform(target);
            performed.Add(target);
        }

        private void Perform(Enemy target)
        {
            sfx.Play();

            float hitRadius = baseRadius + radiusPerLevel * (level - 1);

            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = target.transform.position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            vfx.transform.localScale = Vector3.one * hitRadius;
            Destroy(vfx, 5f);


            Vector3 position = new Vector3(vfx.transform.position.x, vfx.transform.position.y, vfx.transform.position.z + vfx.transform.localScale.z / 2);

            Collider[] hitColliders = Physics.OverlapSphere(position, hitRadius);

            foreach (var hitCollider in hitColliders)
            {
                hitCollider.TryGetComponent(out Enemy enemy);
                if (enemy == null || !enemy.IsAlive)
                    continue;

                float dmg = baseDamage + damagePerLevel * (level - 1);
                Damage damage = new Damage(dmg, Element.Type.Fire, PlayerStats.Instance, enemy);
                PlayerManager.Instance.PerformSkillDamage(enemy, damage);
            }
        }
    }
}
