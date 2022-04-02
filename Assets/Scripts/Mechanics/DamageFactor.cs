using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    [System.Serializable]
    public class DamageFactor
    {
        private ElementDamageFactor physical;
        private ElementDamageFactor fire;
        private ElementDamageFactor cryo;
        private ElementDamageFactor electro;
        private ElementDamageFactor venom;

        [System.Serializable]
        public class ElementDamageFactor
        {
            private float value = 0f;
            private float debuffChance = 0f;
            private Element.Type element;

            public ElementDamageFactor(Element.Type element)
            {
                this.element = element;
            }

            public float Value { get { return value; } set { if (value < 0) value = 0f; this.value = value; } }
            public Element.Type Element { get { return element; } }
            public float DebuffChance { get { return debuffChance; } set { debuffChance = Mathf.Clamp(value, 0f, 1f); } }
        }

        public DamageFactor()
        {
            physical = new ElementDamageFactor(Element.Type.Physical);
            fire = new ElementDamageFactor(Element.Type.Fire);
            cryo = new ElementDamageFactor(Element.Type.Cryo);
            electro = new ElementDamageFactor(Element.Type.Electro);
            venom = new ElementDamageFactor(Element.Type.Venom);
        }

        public void SetDamageValues(float physical, float fire, float cryo, float electro, float venom)
        {
            this.physical.Value = physical;
            this.fire.Value = fire;
            this.cryo.Value = cryo;
            this.electro.Value = electro;
            this.venom.Value = venom;
        }

        public void SetDebuffChances(float burned, float frozen, float electrocuted, float infected)
        {
            fire.DebuffChance = burned;
            cryo.DebuffChance = frozen;
            electro.DebuffChance = electrocuted;
            venom.DebuffChance = infected;
        }

        public ElementDamageFactor FindDamageFactorByElement(Element.Type element)
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

        public ElementDamageFactor Physical { get { return physical; } }
        public ElementDamageFactor Fire { get { return fire; } }
        public ElementDamageFactor Cryo { get { return cryo; } }
        public ElementDamageFactor Electro { get { return electro; } }
        public ElementDamageFactor Venom { get { return venom; } }
    }
}
