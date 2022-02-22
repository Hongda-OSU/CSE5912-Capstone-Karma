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


            bonusList = new List<Bonus>();
            for (int i = 0; i < (int)weapon.Rarity; i++)
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
            internal delegate void BonusFunction(bool add);

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
            private static float critDamageBonus = 0.06f;
            private static float critRateBonus = 0.12f;
            private static float ammoBonus = 0.2f;

            //internal enum 
            internal Bonus(Firearms weapon)
            {
                this.weapon = weapon;
                level = (int)weapon.Rarity;
                isReady = true;

                bonusFunctionList = new List<BonusFunction>()
                { 
                    IncreaseDamage_physical, 
                    IncreaseDamage_fire, 
                    IncreaseDamage_cryo,
                    IncreaseDamage_electro,
                    IncreaseDamage_venom,
                    IncreaseCritDamage,
                    IncreaseCritRate,
                    IncreaseAmmo,
                };
            }

            internal void AssignBonusFunction(int index)
            {
                var func = bonusFunctionList[index];
                bonusFunction = func;
            }

            internal void Perform(bool enabled)
            {
                bonusFunction(enabled);
            }

            private float ResolveValue(float value)
            {
                float resolvedValue = UnityEngine.Random.Range(value * (1 - valueVariance), value * (1 + valueVariance));
                return (float)Math.Round(resolvedValue, 3);
            }
            internal void IncreaseDamage_physical(bool add)
            {
                if (add != isReady)
                    return;

                if (!isInitialized)
                {
                    value = elementDamageBonus * factorPerLevel * (level + 1);
                    value = ResolveValue(value);
                    isInitialized = true;
                }

                description = "Physical Damage +" + Math.Round(value * 100, 1) + "%";

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

                if (!isInitialized)
                {
                    value = elementDamageBonus * factorPerLevel * (level + 1);
                    value = ResolveValue(value);
                    isInitialized = true;
                }

                description = "Fire Damage +" + Math.Round(value * 100, 1) + "%";

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


            internal void IncreaseDamage_cryo(bool add)
            {
                if (add != isReady)
                    return;

                if (!isInitialized)
                {
                    value = elementDamageBonus * factorPerLevel * (level + 1);
                    value = ResolveValue(value);
                    isInitialized = true;
                }

                description = "Cryo Damage +" + Math.Round(value * 100, 1) + "%";

                if (add)
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

            internal void IncreaseDamage_electro(bool add)
            {
                if (add != isReady)
                    return;

                if (!isInitialized)
                {
                    value = elementDamageBonus * factorPerLevel * (level + 1);
                    value = ResolveValue(value);
                    isInitialized = true;
                }

                description = "Electro Damage +" + Math.Round(value * 100, 1) + "%";

                if (add)
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

            internal void IncreaseDamage_venom(bool add)
            {
                if (add != isReady)
                    return;

                if (!isInitialized)
                {
                    value = elementDamageBonus * factorPerLevel * (level + 1);
                    value = ResolveValue(value);
                    isInitialized = true;
                }

                description = "Venom Damage +" + Math.Round(value * 100, 1) + "%";

                if (add)
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


            internal void IncreaseCritDamage(bool add)
            {
                if (add != isReady)
                    return;

                if (!isInitialized)
                {
                    value = critDamageBonus * factorPerLevel * (level + 1);
                    value = ResolveValue(value);
                    isInitialized = true;
                }

                description = "Critical Damage +" + Math.Round(value * 100, 1) + "%";

                if (add)
                {
                    PlayerStats.Instance.CritDamageFactor += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.CritDamageFactor -= value;
                    isReady = true;
                }
            }

            internal void IncreaseCritRate(bool add)
            {
                if (add != isReady)
                    return;

                if (!isInitialized)
                {
                    value = critRateBonus * factorPerLevel * (level + 1);
                    value = ResolveValue(value);
                    isInitialized = true;
                }

                description = "Critical Rate +" + Math.Round(value * 100, 1) + "%";

                if (add)
                {
                    PlayerStats.Instance.CritRate += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.CritRate -= value;
                    isReady = true;
                }
            }

            internal void IncreaseAmmo(bool add)
            {
                if (add != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ammoBonus * factorPerLevel * (level + 1);
                    value = ResolveValue(value);
                    isInitialized = true;
                }

                description = "Ammo +" + Math.Round(value * 100, 1) + "%";

                int ammo = WeaponManager.Instance.CarriedWeapon.AmmoInMag;
                if (add)
                {
                    WeaponManager.Instance.CarriedWeapon.AmmoInMag = (int)Mathf.Floor(ammo * (1 + value));
                    isReady = false;
                }
                else
                {
                    WeaponManager.Instance.CarriedWeapon.AmmoInMag = (int)Mathf.Ceil(ammo / (1 + value));
                    isReady = true;
                }
            }



            public string Description { get { return description; } }
        } 
    }
}
