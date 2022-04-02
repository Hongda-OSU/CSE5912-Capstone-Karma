using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    [System.Serializable]
    public class Resist
    {
        private ElementResist physical;
        private ElementResist fire;
        private ElementResist cryo;
        private ElementResist electro;
        private ElementResist venom;

        [System.Serializable]
        public class ElementResist
        {
            private float value = 0f;
            private Element.Type element;

            public ElementResist(Element.Type element)
            {
                this.element = element;
            }

            public float Value { get { return value; } set { if (value < 0) value = 0f; this.value = value; } }
            public Element.Type Element { get { return element; } }
        }

        public Resist()
        {
            physical = new ElementResist(Element.Type.Physical);
            fire = new ElementResist(Element.Type.Fire);
            cryo = new ElementResist(Element.Type.Cryo);
            electro = new ElementResist(Element.Type.Electro);
            venom = new ElementResist(Element.Type.Venom);
        }

        public void SetValues(float physical, float fire, float cryo, float electro, float venom)
        {
            this.physical.Value = physical;
            this.fire.Value = fire;
            this.cryo.Value = cryo;
            this.electro.Value = electro;
            this.venom.Value = venom;
        }

        public ElementResist FindResisByElement(Element.Type element)
        {
            switch (element)
            {
                case Element.Type.Physical:
                    return physical;
                case Element.Type.Fire:
                    return fire;
                case Element.Type.Cryo:
                    return cryo;
                case Element.Type.Electro:
                    return electro;
                case Element.Type.Venom:
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
