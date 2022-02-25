using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Electrocuted : Debuff
    {
        [SerializeField] private float reductionPerStack = 0.05f;
        private float deltaTime = 0.1f;



        protected override IEnumerator Perform()
        {
            while (timeSince < duration)
            {
                timeSince += deltaTime;
                yield return new WaitForSeconds(deltaTime);

                target.GetResist().Physical.Value = target.PhysicalResist * (1 - reductionPerStack * stack);
                target.GetResist().Fire.Value = target.FireResist * (1 - reductionPerStack * stack);
                target.GetResist().Cryo.Value = target.CryoResist * (1 - reductionPerStack * stack);
                target.GetResist().Electro.Value = target.ElectroResist * (1 - reductionPerStack * stack);
                target.GetResist().Venom.Value = target.VenomResist * (1 - reductionPerStack * stack);
            }
            stack = 0;
        }
    }
}
