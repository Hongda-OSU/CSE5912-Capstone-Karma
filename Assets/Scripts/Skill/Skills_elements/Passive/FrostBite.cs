using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Frostbite : Skill
    {

        [Header("Frostbite")]
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

            float hitRadius = baseRadius + radiusPerLevel * (level - 1);

            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = PlayerManager.Instance.Player.transform.position;
            vfx.transform.localScale = Vector3.one * hitRadius / 8f; // hard coded
            Destroy(vfx, 1f);

            Vector3 position = new Vector3(vfx.transform.position.x, vfx.transform.position.y, vfx.transform.position.z + vfx.transform.localScale.z / 2);

            Collider[] hitColliders = Physics.OverlapSphere(position, hitRadius);

            float dmg = baseDamage + damagePerLevel * (level - 1);
            foreach (var hitCollider in hitColliders)
            {
                hitCollider.TryGetComponent(out Enemy enemy);
                if (enemy == null || !enemy.IsAlive)
                    continue;

                Damage damage = new Damage(dmg, Element.Type.Cryo, PlayerStats.Instance, enemy);
                PlayerManager.Instance.PerformSkillDamage(enemy, damage);
                enemy.Frozen.StackUp();
            }
        }
    }
}
