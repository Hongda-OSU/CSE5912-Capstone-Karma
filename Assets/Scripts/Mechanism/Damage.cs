using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Damage
    {
        private float rawValue;
        private float resolvedValue;

        private bool isCrit = false;

        private Element.Type element;

        private IDamageable source;
        private IDamageable target;

        public Damage(float rawValue, Element.Type element, IDamageable source, IDamageable target)
        {
            float extra = source.ComputeExtraDamage(rawValue);
            if (extra > 0f)
                isCrit = true;

            this.rawValue = rawValue + extra;
            this.element = element;
            this.source = source;
            this.target = target;
            this.resolvedValue = CalculateResolvedValue(source.GetDamageFactor(), target.GetResist());
        }


        private float CalculateResolvedValue(DamageFactor damageFactor, Resist resist)
        {
            float damageFactorValue = damageFactor.FindDamageFactorByElement(element).Value;
            float resistValue = resist.FindResisByElement(element).Value;

            return rawValue * (1 + damageFactorValue) * (1 - PercentageReduced(resistValue));
        }

        public static float PercentageReduced(float resistValue)
        {
            float value = 1 - 100 / (100 + resistValue);

            return (float)System.Math.Round(value, 2);
        }

        public float RawValue { get { return rawValue; } }
        public float ResolvedValue { get { return resolvedValue; } }
        public bool IsCrit { get { return isCrit; } }
        public Element.Type Element { get { return element; } }
        public IDamageable Source { get { return source; } }
        public IDamageable Target { get { return target; } }
    }
}
