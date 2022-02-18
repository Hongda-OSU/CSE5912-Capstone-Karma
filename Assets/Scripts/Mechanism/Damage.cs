using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Damage
    {
        private float value;

        private ElementType element;

        private GameObject source;
        private GameObject target;


        public enum ElementType
        {
            None,
            Fire,
            Cryo,
            Electro,
            Venom,
        }

        public Damage(float value, ElementType element, GameObject source, GameObject target)
        {
            this.value = value;
            this.element = element;
            this.source = source;
            this.target = target;
        }

        public float Value { get { return value; } set { this.value = value; } }
        public ElementType Element { get { return element; } set { element = value; } }
        public GameObject Source { get { return source; } set { source = value; } }
        public GameObject Target { get { return target; } set { target = value; } }
    }
}
