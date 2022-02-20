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

        private ElementType element;

        private IDamageable source;
        private IDamageable target;


        public enum ElementType
        {
            Physical,
            Fire,
            Cryo,
            Electro,
            Venom,
        }

        public Damage(float rawValue, ElementType element, IDamageable source, IDamageable target)
        {
            float extra = source.ComputeExtraDamage();
            if (extra > 0f)
                isCrit = true;

            this.rawValue = rawValue + extra;
            this.element = element;
            this.source = source;
            this.target = target;
            this.resolvedValue = CalculateResolvedValue(target.GetResist());
        }

        private float CalculateResolvedValue(Resist resist)
        {

            float resistValue = resist.FindResisByElement(element).Value;

            return rawValue * (100 / (100 + resistValue));
        }

        public float RawValue { get { return rawValue; } }
        public float ResolvedValue { get { return resolvedValue; } }
        public bool IsCrit { get { return isCrit; } }
        public ElementType Element { get { return element; } }
        public IDamageable Source { get { return source; } }
        public IDamageable Target { get { return target; } }
    }
}
