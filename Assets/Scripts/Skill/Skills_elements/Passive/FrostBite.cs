using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FrostBite : Skill
    {

        [Header("FrostBite")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float baseDamage = 20f;
        [SerializeField] private float damagePerLevel = 10f;

        [SerializeField] private float baseRadius = 3f;
        [SerializeField] private float radiusPerLevel = 1f;

        private void Start()
        {
            PlayerStats.Instance.TakeDamageEvent.AddListener(Perform);
        }

        private void Perform()
        {
            if (!isLearned || !isReady)
                return;

            StartCoolingdown();

            float radius = baseRadius + radiusPerLevel * (level - 1);

            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = PlayerManager.Instance.Player.transform.position;
            vfx.transform.localScale = Vector3.one * radius / 6f; // hard coded
            Destroy(vfx, 1f);

            float dmg = baseDamage + damagePerLevel * (level - 1);
            foreach (var target in EnemyManager.Instance.EnemyList)
            {
                float distance = Vector3.Distance(target.transform.position, target.transform.position);
                if (distance < radius)
                {
                    Enemy enemy = target.GetComponent<Enemy>();
                    Damage damage = new Damage(dmg, Element.Type.Cryo, PlayerStats.Instance, enemy);
                    PlayerManager.Instance.PerformSkillDamage(enemy, damage);
                    enemy.Frozen.StackUp();
                }
            }
        }
    }
}
