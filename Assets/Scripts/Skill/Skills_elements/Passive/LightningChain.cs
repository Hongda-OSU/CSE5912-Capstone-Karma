using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LightningChain : Skill
    {
        [Header("Lightning Chain")]
        [SerializeField] private GameObject lightningPrefab;
        [SerializeField] private float displayTime = 0.3f;

        [SerializeField] private float effectDistance = 10f;

        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float damagePerLevel = 10f;

        [SerializeField] private int baseLightningNumber = 1;
        [SerializeField] private int lightningNumberPerLevel = 1;

        private void Update()
        {
            var target = PlayerManager.Instance.HitByBullet;
            if (!isLearned || target == null || !target.IsAlive)
                return;


            int lightningNum = baseLightningNumber + lightningNumberPerLevel * (level - 1);

            List<GameObject> inRange = new List<GameObject>();


            foreach (var enemy in EnemyManager.Instance.EnemyList)
            {
                float distance = Vector3.Distance(target.gameObject.transform.position, enemy.transform.position);
                if (distance < effectDistance && enemy != target.gameObject)
                {
                    inRange.Add(enemy);
                }
            }

            Enemy targetEnemy = target.GetComponent<Enemy>();
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

            PlayerManager.Instance.HitByBullet = null;
        }

        private IEnumerator DisplayVfx(Vector3 first, Vector3 second)
        {
            GameObject vfx = Instantiate(lightningPrefab);

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

            Destroy(vfx);
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
