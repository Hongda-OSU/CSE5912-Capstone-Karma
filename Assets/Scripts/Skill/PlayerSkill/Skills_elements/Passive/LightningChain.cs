using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LightningChain : PlayerSkill
    {
        [Header("Lightning Chain")]
        [SerializeField] private GameObject vfxPrefab;
        [SerializeField] private float displayTime = 0.3f;

        [SerializeField] private float effectDistance = 10f;

        [SerializeField] private float baseChance = 0.3f;
        [SerializeField] private float chancePerLevel = 0.1f;

        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float damagePerLevel = 10f;

        [SerializeField] private int baseLightningNumber = 1;
        [SerializeField] private int lightningNumberPerLevel = 1;

        [SerializeField] private AudioSource sfx;

        protected override string GetBuiltSpecific()
        {
            var dmg = BuildSpecific("Damage", baseDamage, damagePerLevel, "", "Cryo damage");
            var chance = BuildSpecific("Chance", baseChance * 100, chancePerLevel * 100, "%", "");
            var num = BuildSpecific("Number of Targets", baseLightningNumber, lightningNumberPerLevel, "", "");
            return dmg + chance + num;
        }
        private void Update()
        {
            var target = PlayerManager.Instance.HitByBullet;
            if (!isLearned || target == null || !target.IsAlive)
                return;

            if (target.Electrocuted.Stack == 0)
                return;

            float chance = baseChance + chancePerLevel * (level - 1);
            if (Random.value > chance)
            {
                PlayerManager.Instance.HitByBullet = null;
                return;
            }

            int lightningNum = baseLightningNumber + lightningNumberPerLevel * (level - 1);

            var position = target.transform.position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            var size = new Vector3(effectDistance, effectDistance, effectDistance);

            Collider[] hitColliders = Physics.OverlapBox(position, size);

            List<GameObject> inRange = new List<GameObject>();
            foreach (var hitCollider in hitColliders)
            {
                hitCollider.TryGetComponent(out Enemy enemy);
                if (enemy == null || !enemy.IsAlive)
                    continue;

                inRange.Add(enemy.gameObject);
            }

            sfx.transform.position = transform.position;

            if (inRange.Count == 0)
                return;

            Enemy targetEnemy = target.GetComponent<Enemy>();

            sfx.transform.position = targetEnemy.transform.position;
            sfx.Play();

            for (int i = 0; i < lightningNum; i++)
            {
                int index = Random.Range(0, inRange.Count);
                Enemy otherEnemy = inRange[index].GetComponent<Enemy>();
                inRange.RemoveAt(index);

                float damageOnTarget = baseDamage + damagePerLevel * (level - 1);
                if (i > 0)
                    damageOnTarget = 0f;

                Perform(targetEnemy, otherEnemy, damageOnTarget);
            }

            PlayerManager.Instance.HitByBullet = null;

        }

        private void Perform(Enemy target, Enemy other, float damageOnTarget)
        {

            Vector3 first = target.transform.position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            Vector3 second = other.transform.position + Vector3.up * other.GetComponentInChildren<Renderer>().bounds.size.y / 2;

            StartCoroutine(DisplayVfx(first, second));

            Damage damage = new Damage(damageOnTarget, Element.Type.Electro, PlayerStats.Instance, target);
            PlayerManager.Instance.PerformSkillDamage(target, damage);

            damage = new Damage(baseDamage, Element.Type.Electro, PlayerStats.Instance, other.GetComponent<Enemy>());
            PlayerManager.Instance.PerformSkillDamage(other.GetComponent<Enemy>(), damage);
        }

        private IEnumerator DisplayVfx(Vector3 first, Vector3 second)
        {
            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = (first + second) / 2;

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

            Destroy(vfx, 5f);
        }

        //public override bool LevelUp()
        //{
        //    bool result = base.LevelUp();
        //    if (result)
        //        isReady = true;

        //    return result;
        //}

        //public override void ResetLevel()
        //{
        //    base.ResetLevel();
        //}
    }
}
