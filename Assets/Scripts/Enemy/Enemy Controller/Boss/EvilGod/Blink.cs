using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Blink : EnemySkill
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform pivot;

        public override IEnumerator Perform()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Perform(Vector3 position)
        {
            GameObject blinks = new GameObject("Blinks");

            var origin = Instantiate(prefab, blinks.transform);
            origin.transform.position = pivot.position;
            Destroy(origin, 5f);


            if (enemy.Animator.applyRootMotion)
            {
                enemy.Animator.applyRootMotion = false;
                yield return new WaitForSeconds(Time.deltaTime);
                enemy.transform.position = position;
                enemy.Animator.applyRootMotion = true;
            }
            else
            {
                yield return new WaitForSeconds(Time.deltaTime);
                enemy.transform.position = position;
            }


            enemy.transform.LookAt(PlayerManager.Instance.Player.transform);

            var target = Instantiate(prefab, blinks.transform);
            target.transform.position = pivot.position;
            Destroy(target, 5f);

            Destroy(blinks, 5f);
        }

        public override bool IsPerformingAllowed()
        {
            return true;
        }

    }
}
