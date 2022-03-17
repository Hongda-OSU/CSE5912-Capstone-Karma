using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class WeaponBonus
    {
        private Firearms weapon;

        private List<int> availableBonusIndex;
        private List<Bonus> bonusList;
        public WeaponBonus(Firearms weapon)
        {
            this.weapon = weapon; 
            Initialize();
        }

        public void Initialize()
        {
            weapon.WeaponName = weapon.Rarity.ToString() + weapon.Type;

            bonusList = new List<Bonus>();
            for (int i = 0; i < (int)weapon.Rarity + 1; i++)
            {
                Bonus bonus = new Bonus(weapon);
                if (i == 0)
                {
                    availableBonusIndex = new List<int>();
                    for (int j = 0; j < Bonus.bonusFunctionList.Count; j++)
                    {
                        availableBonusIndex.Add(j);
                    }
                }

                int index = availableBonusIndex[UnityEngine.Random.Range(0, availableBonusIndex.Count)];
                bonus.AssignBonusFunction(index);
                availableBonusIndex.Remove(index);

                bonusList.Add(bonus);
            }
        }

        public void Perform(bool enabled)
        {
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
            internal delegate void BonusFunction(bool enabled);

            private Firearms weapon;

            private float value;
            private int level;
            internal string description = "None";
            private bool isReady;
            private bool isInitialized = false;

            private BonusFunction bonusFunction;
            internal static List<BonusFunction> bonusFunctionList;

            private static float factorPerLevel = 0.5f;
            private static float valueVariance = 0.3f;

            private static float elementDamageBonus = 0.042f;
            private static float fireRateBonus = 0.05f;

            //internal enum 
            internal Bonus(Firearms weapon)
            {
                this.weapon = weapon;
                level = (int)weapon.Rarity + 1;
                isReady = true;

                bonusFunctionList = new List<BonusFunction>()
                { 
                    IncreaseDamage_physical, 
                    IncreaseDamage_fire, 
                    IncreaseDamage_cryo,
                    IncreaseDamage_electro,
                    IncreaseDamage_venom,
                    IncreaseFireRate,
                };
            }

            internal void AssignBonusFunction(int index)
            {
                var func = bonusFunctionList[index];

                func(true);
                func(false);

                bonusFunction = func;
            }

            internal void Perform(bool enabled)
            {
                bonusFunction(enabled);
            }

            private float ResolveValue(float bonusValue)
            {
                float resolvedValue = bonusValue * factorPerLevel * (level + 1);

                resolvedValue = UnityEngine.Random.Range(resolvedValue * (1 - valueVariance), resolvedValue * (1 + valueVariance));

                return (float)Math.Round(resolvedValue, 3);
            }


            /*
             *  Regular Bonuses
             */

            internal void IncreaseDamage_physical(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(elementDamageBonus);
                    isInitialized = true;
                }

                description = "Physical Damage +" + Math.Round(value * 100, 1) + "%";

                if (enabled)
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

            internal void IncreaseDamage_fire(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(elementDamageBonus);
                    isInitialized = true;
                }

                description = "Fire Damage +" + Math.Round(value * 100, 1) + "%";

                if (enabled)
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


            internal void IncreaseDamage_cryo(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(elementDamageBonus);
                    isInitialized = true;
                }

                description = "Cryo Damage +" + Math.Round(value * 100, 1) + "%";

                if (enabled)
                {
                    PlayerStats.Instance.GetDamageFactor().Cryo.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetDamageFactor().Cryo.Value -= value;
                    isReady = true;
                }
            }

            internal void IncreaseDamage_electro(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(elementDamageBonus);
                    isInitialized = true;
                }

                description = "Electro Damage +" + Math.Round(value * 100, 1) + "%";

                if (enabled)
                {
                    PlayerStats.Instance.GetDamageFactor().Electro.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetDamageFactor().Electro.Value -= value;
                    isReady = true;
                }
            }

            internal void IncreaseDamage_venom(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(elementDamageBonus);
                    isInitialized = true;
                }

                description = "Venom Damage +" + Math.Round(value * 100, 1) + "%";

                if (enabled)
                {
                    PlayerStats.Instance.GetDamageFactor().Venom.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetDamageFactor().Venom.Value -= value;
                    isReady = true;
                }
            }

            internal void IncreaseFireRate(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(fireRateBonus);
                    isInitialized = true;
                }

                description = "Fire Rate +" + Math.Round(value * 100, 1) + "%";

                if (enabled)
                {
                    weapon.FireRate *= 1 + value;
                    isReady = false;
                }
            }

            public string Description { get { return description; } }
        } 
    }
}
