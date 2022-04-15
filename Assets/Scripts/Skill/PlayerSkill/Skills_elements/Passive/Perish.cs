using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Perish : PlayerSkill
    {

        [Header("Perish")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float baseTriggerHealth = 0.05f;
        [SerializeField] private float triggerHealthPerLevel = 0.015f;


        protected override string GetBuiltSpecific()
        {
            var dmg = BuildSpecific("Execution Health", baseTriggerHealth * 100, triggerHealthPerLevel * 100, "%", "of target's max health");
            return dmg;
        }

        private void Update()
        {
            var target = PlayerManager.Instance.HitByBullet;
            if (!isLearned || target == null || !target.IsAlive)
                return;

            bool isTriggered = target.Health / target.MaxHealth <= baseTriggerHealth + triggerHealthPerLevel * (level - 1);
            if (target.Infected.Stack < target.Infected.MaxStack || !isTriggered)
                return;

            Perform(target);
        }

        private void Perform(Enemy target)
        {
            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = target.transform.position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            Destroy(vfx, 5f);

            float dmg = target.Health * 10f;
            Damage damage = new Damage(dmg, Element.Type.Venom, PlayerStats.Instance, target);
            PlayerManager.Instance.PerformSkillDamage(target, damage);
        }
    }
}
