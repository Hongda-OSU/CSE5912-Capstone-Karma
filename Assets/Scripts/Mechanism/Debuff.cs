using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Debuff
    {
        private IDamageable source;
        private IDamageable target;

        private int burned = 0;
        private int frozen = 0;
        private int electrocuted = 0;
        private int infected = 0;

        private int maxBurned = 5;
        private int maxFrozen = 5;
        private int maxElectrocuted = 5;
        private int maxInfected = 5;

        public enum DebuffType
        {
            Burned,
            Frozen,
            Electrocuted,
            Infected,
        }

        public Debuff(IDamageable source, IDamageable target)
        {
            this.source = source;
            this.target = target;
        }


        public int GetDebuffStack(DebuffType type)
        {
            switch (type)
            {
                case DebuffType.Burned:
                    return burned;
                case DebuffType.Frozen:
                    return frozen;
                case DebuffType.Electrocuted:
                    return electrocuted;
                case DebuffType.Infected:
                    return infected;
            }
            Debug.LogError("Error: Debuff type does not exist. ");
            return -1;
        }


        public IDamageable Source { get { return source; } }
        public IDamageable Target { get { return target; } }

        public int Burned { get { return burned; } set { burned = Mathf.Clamp(value, 0, maxBurned); } }
        public int Frozen { get { return frozen; } set { frozen = Mathf.Clamp(value, 0, maxFrozen); } }
        public int Electrocuted { get { return electrocuted; } set { Mathf.Clamp(value, 0, maxElectrocuted); } }
        public int Infected { get { return infected; } set { infected = Mathf.Clamp(value, 0, maxInfected); } }


        public int MaxBurned { get { return maxBurned; } set { maxBurned = value; } }
        public int MaxFrozen { get { return maxFrozen; } set { MaxFrozen = value; } }
        public int MaxElectrocuted { get { return maxElectrocuted; } set { maxElectrocuted = value; } }
        public int MaxInfected { get { return maxInfected; } set { maxInfected = value; } }
    }
}
