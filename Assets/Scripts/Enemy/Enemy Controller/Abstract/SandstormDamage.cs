using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class SandstormDamage : MonoBehaviour
    {
        [SerializeField] private BossArea area;
        [SerializeField] private Transform safeRangeCenter;
        [SerializeField] private float safeRangeRadius;
        [SerializeField] private BossEnemy boss;

        void Update()
        {
            if (area.GetIsTriggered() && !area.GetIsBossDefeated())
            {
                if (Vector3.Distance(PlayerManager.Instance.Player.transform.position, safeRangeCenter.position) > safeRangeRadius) 
                {
                    PlayerStats.Instance.TakeDamage(new Damage(5f, Element.Type.Physical, boss, PlayerStats.Instance));
                }
            }

        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(safeRangeCenter.position, safeRangeRadius);
        }
#endif
    }
}
