using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class WeaponBonus
    {
        private List<Bonus> bonusList;
        public WeaponBonus(int weaponLevel)
        {
            bonusList = new List<Bonus>();
            for (int i = 0; i < weaponLevel; i++)
            {
                bonusList.Add(new Bonus(weaponLevel));
            }
        }

        public void Perform(bool enabled)
        {
            if (!enabled)
            {
                int i = 3;
            }
            foreach (var bonus in bonusList)
                bonus.Perform(enabled);
        }

        public List<string> GetBonusDescriptionList()
        {
            var list = new List<string>();
            foreach(var bonus in bonusList)
                list.Add(bonus.Description);
            return list;
        }


        internal class Bonus
        {
            private delegate void BonusFunction(bool add);

            private int level;
            internal string description = "None";
            private bool isReady;
            private BonusFunction bonusFunction;

            private static float factorPerLevel = 0.5f;
            private static float damageBonus = 0.042f;

            internal Bonus(int level)
            {
                this.level = level;
                isReady = true;

                BonusFunction[] bonusFunctions = new BonusFunction[] { IncreaseDamage_physical, IncreaseDamage_fire };
                bonusFunction = bonusFunctions[Random.Range(0, bonusFunctions.Length)];
            }

            internal void Perform(bool enabled)
            {
                bonusFunction(enabled);
            }

            internal void IncreaseDamage_physical(bool add)
            {
                if (add != isReady)
                    return;

                float value = damageBonus * factorPerLevel * (level + 1);
                description = "Physical Damage +" + value * 100 + "%";

                if (add)
                {
                    PlayerStats.Instance.GetDamageFactor().Physical.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetDamageFactor().Physical.Value -= value;
                    isReady = true;
                }
            }

            internal void IncreaseDamage_fire(bool add)
            {
                if (add != isReady)
                    return;

                float value = damageBonus * factorPerLevel * (level + 1);
                description = "Fire Damage +" + value * 100 + "%";

                if (add)
                {
                    PlayerStats.Instance.GetDamageFactor().Fire.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetDamageFactor().Fire.Value -= value;
                    isReady = true;
                }
            }

            public string Description { get { return description; } }
        } 
    }
}
