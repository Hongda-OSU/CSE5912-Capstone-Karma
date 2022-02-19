using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Damage
    {
        private float rawValue;

        private float resolvedValue;

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
            this.rawValue = rawValue;
            this.element = element;
            this.source = source;
            this.target = target;
            this.resolvedValue = CalculateResolvedValue(target.GetResist());
            Debug.Log(resolvedValue);
        }

        private float CalculateResolvedValue(Resist resist)
        {
            float resistValue = resist.FindResisByElement(element).Value;
            Debug.Log(resistValue);
            return rawValue * (100 / (100 + resistValue));
        }

        public float RawValue { get { return rawValue; } }
        public float ResolvedValue { get { return resolvedValue; } }
        public ElementType Element { get { return element; } }
        public IDamageable Source { get { return source; } }
        public IDamageable Target { get { return target; } }
    }
}
