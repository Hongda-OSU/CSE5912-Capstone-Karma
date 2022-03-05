using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Detonation : Skill
    {
        [Header("Detonation")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float triggerTime = 5f;

        [SerializeField] private float baseDamage = 200f;
        [SerializeField] private float damagePerLevel = 50f;
        

        private void Update()
        {
            var target = PlayerManager.Instance.HitByBullet;
            if (!isLearned || target == null || !target.IsAlive)
                return;

            if (target.Electrocuted.Stack != target.Electrocuted.MaxStack)
                return;

            StartCoroutine(Perform(target));
        }

        private IEnumerator Perform(Enemy target)
        {
            if (!isReady)
                yield break;
            isReady = false;

            yield return new WaitForSeconds(triggerTime);

            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = target.transform.position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            Destroy(vfx, 5f);

            float dmg = baseDamage + damagePerLevel * (level - 1);
            Damage damage = new Damage(dmg, Element.Type.Electro, PlayerStats.Instance, target);
            PlayerManager.Instance.PerformSkillDamage(target, damage);

            isReady = true;
            target.Electrocuted.Stack = 0;
        }
    }
}
