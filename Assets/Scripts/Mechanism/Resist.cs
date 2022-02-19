using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Resist
    {
        private ElementResist physical;
        private ElementResist fire;
        private ElementResist cryo;
        private ElementResist electro;
        private ElementResist venom;

        public class ElementResist
        {
            private float value = 0f;
            private Damage.ElementType element;

            public ElementResist(Damage.ElementType element)
            {
                this.element = element;
            }

            public float Value { get { return value; } set { if (value >= 0f) this.value = value; } }
            public Damage.ElementType Element { get { return element; } }
        }

        public Resist()
        {
            physical = new ElementResist(Damage.ElementType.Physical);
            fire = new ElementResist(Damage.ElementType.Fire);
            cryo = new ElementResist(Damage.ElementType.Cryo);
            electro = new ElementResist(Damage.ElementType.Electro);
            venom = new ElementResist(Damage.ElementType.Venom);
        }

        public void SetValues(float physical, float fire, float cryo, float electro, float venom)
        {
            this.physical.Value = physical;
            this.fire.Value = fire;
            this.cryo.Value = cryo;
            this.electro.Value = electro;
            this.venom.Value = venom;
        }

        public ElementResist FindResisByElement(Damage.ElementType element)
        {
            switch (element)
            {
                case Damage.ElementType.Physical:
                    return physical;
                case Damage.ElementType.Fire:
                    return fire;
                case Damage.ElementType.Cryo:
                    return cryo;
                case Damage.ElementType.Electro:
                    return electro;
                case Damage.ElementType.Venom:
                    return venom;
            }
            return null;
        }

        public ElementResist Physical { get { return physical; } }
        public ElementResist Fire { get { return fire; } }
        public ElementResist Cryo { get { return cryo; } }
        public ElementResist Electro { get { return electro; } }
        public ElementResist Venom { get { return venom; } }
    }
}
