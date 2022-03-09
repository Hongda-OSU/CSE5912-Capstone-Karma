using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class MixedInfection : Skill
    {

        [Header("MixedInfection")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private int baseMaxStack = 6;
        [SerializeField] private int maxStackPerLevel = 1;

        [SerializeField] private float baseTime = 6f;
        [SerializeField] private float timePerLevel = 1f;

        private void Update()
        {
            var target = PlayerManager.Instance.HitByBullet;
            if (!isLearned || target == null || !target.IsAlive)
                return;

            Perform(target);
        }

        private void Perform(Enemy target)
        {
            //GameObject vfx = Instantiate(vfxPrefab);
            //vfx.transform.position = target.transform.position + Vector3.up * (target.GetComponentInChildren<Renderer>().bounds.size.y + 1f);
            //Destroy(vfx, 5f);

            int max = baseMaxStack + maxStackPerLevel * (level - 1);
            target.Infected.MaxStack = max;

            float time = baseTime + timePerLevel * (level - 1);
            target.Infected.Duration = time;
        }
    }
}
