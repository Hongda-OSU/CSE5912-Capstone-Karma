using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Electrocuted : Debuff
    {
        private float deltaTime = 0.1f;



        protected override IEnumerator Perform()
        {
            while (timeSince < duration)
            {
                timeSince += deltaTime;
                yield return new WaitForSeconds(deltaTime);

                SetResistValues();
            }
            stack = 0;
            SetResistValues();
        }

        private void SetResistValues()
        {
            float final = 1 - PlayerStats.Instance.ElectrocutedResistReductionPerStack * stack;

            target.GetResist().Physical.Value = target.PhysicalResist * final;
            target.GetResist().Fire.Value = target.FireResist * final;
            target.GetResist().Cryo.Value = target.CryoResist * final;
            target.GetResist().Electro.Value = target.ElectroResist * final;
            target.GetResist().Venom.Value = target.VenomResist * final;
        }
    }
}
