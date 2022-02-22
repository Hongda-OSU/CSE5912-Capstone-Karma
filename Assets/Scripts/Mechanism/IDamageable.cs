using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public interface IDamageable
    {
        public void TakeDamage(Damage damage);

        public DamageFactor GetDamageFactor();
        public Resist GetResist();

        public float ComputeExtraDamage();
    }
}
