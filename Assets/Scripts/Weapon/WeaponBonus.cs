using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    [Serializable]
    public class WeaponBonus
    {
        private int level;

        private List<int> availableBonusIndex;
        private List<Bonus> bonusList;

        public WeaponBonus(Firearms.WeaponRarity rarity)
        {
            level = (int)rarity + 1;

            Initialize();
        }

        public void Initialize()
        {
            bonusList = new List<Bonus>();
            for (int i = 0; i < level; i++)
            {
                Bonus bonus = new Bonus(level);
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

                if (availableBonusIndex.Count == 0)
                    break;
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

        [Serializable]
        internal class Bonus
        {
            internal delegate void BonusFunction(bool enabled);

            private float value;

            private int level;
            internal string description = "None";
            private bool isReady;
            private bool isInitialized = false;

            private BonusFunction bonusFunction;
            internal static List<BonusFunction> bonusFunctionList;

            private static float factorPerLevel = 0.5f;
            private static float valueVariance = 0.3f;

            // bonus parameters
            private static float elementDamageBonus = 0.042f;
            private static float elementResistBonus = 20f;
            private static float fireRateBonus = 0.05f;
            private static float meleeDamageBonus = 0.5f;
            private static float meleeSpeedBonus = 0.2f;
            private static float moveSpeedBonus = 0.1f;
            private static float experienceBonus = 0.3f;


            //internal enum 
            internal Bonus(int level)
            {
                this.level = level;

                isReady = true;

                bonusFunctionList = new List<BonusFunction>()
                {
                    IncreaseDamage_physical,
                    IncreaseDamage_fire,
                    IncreaseDamage_cryo,
                    IncreaseDamage_electro,
                    IncreaseDamage_venom,

                    IncreaseResist_physical,
                    IncreaseResist_fire,
                    IncreaseResist_cryo,
                    IncreaseResist_electro,
                    IncreaseResist_venom,

                    IncreaseFireRate,

                    IncreaseMeleeDamage,
                    IncreaseMeleeSpeed,

                    IncreaseMoveSpeed,

                    IncreaseExperience,

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
                if (enabled != isReady)
                    return;

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
                if (!isInitialized)
                {
                    value = ResolveValue(elementDamageBonus);
                    isInitialized = true;
                }

                description = RichTextHelper.WrapInColor("Fire", Element.Type.Fire) + " Damage +" + Math.Round(value * 100, 1) + "%";

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
                if (!isInitialized)
                {
                    value = ResolveValue(elementDamageBonus);
                    isInitialized = true;
                }

                description = RichTextHelper.WrapInColor("Cryo", Element.Type.Cryo) + " Damage +" + Math.Round(value * 100, 1) + "%";

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
                if (!isInitialized)
                {
                    value = ResolveValue(elementDamageBonus);
                    isInitialized = true;
                }

                description = RichTextHelper.WrapInColor("Electro", Element.Type.Electro) + " Damage +" + Math.Round(value * 100, 1) + "%";

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
                if (!isInitialized)
                {
                    value = ResolveValue(elementDamageBonus);
                    isInitialized = true;
                }

                description = RichTextHelper.WrapInColor("Venom", Element.Type.Venom) + " Damage +" + Math.Round(value * 100, 1) + "%";

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

            internal void IncreaseResist_physical(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(elementResistBonus);
                    isInitialized = true;
                }

                description = "Physical Resist +" + Math.Round(value, 1);

                if (enabled)
                {
                    PlayerStats.Instance.GetResist().Physical.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetResist().Physical.Value -= value;
                    isReady = true;
                }
            }

            internal void IncreaseResist_fire(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(elementResistBonus);
                    isInitialized = true;
                }

                description = RichTextHelper.WrapInColor("Fire", Element.Type.Fire) + " Resist +" + Math.Round(value, 1);

                if (enabled)
                {
                    PlayerStats.Instance.GetResist().Fire.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetResist().Fire.Value -= value;
                    isReady = true;
                }
            }

            internal void IncreaseResist_cryo(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(elementResistBonus);
                    isInitialized = true;
                }

                description = RichTextHelper.WrapInColor("Cryo", Element.Type.Cryo) + " Resist +" + Math.Round(value, 1);

                if (enabled)
                {
                    PlayerStats.Instance.GetResist().Cryo.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetResist().Cryo.Value -= value;
                    isReady = true;
                }
            }
            internal void IncreaseResist_electro(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(elementResistBonus);
                    isInitialized = true;
                }

                description = RichTextHelper.WrapInColor("Electro", Element.Type.Electro) + " Resist +" + Math.Round(value, 1);

                if (enabled)
                {
                    PlayerStats.Instance.GetResist().Electro.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetResist().Electro.Value -= value;
                    isReady = true;
                }
            }
            internal void IncreaseResist_venom(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(elementResistBonus);
                    isInitialized = true;
                }

                description = RichTextHelper.WrapInColor("Venom", Element.Type.Venom) + " Resist +" + Math.Round(value, 1);

                if (enabled)
                {
                    PlayerStats.Instance.GetResist().Venom.Value += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetResist().Venom.Value -= value;
                    isReady = true;
                }
            }

            internal void IncreaseFireRate(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(fireRateBonus);
                    description = "Fire Rate +" + Math.Round(value * 100, 1) + "%";

                  isInitialized = true;
                }


                if (enabled)
                {
                    PlayerStats.Instance.FireRateFactor += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.FireRateFactor -= value;
                    isReady = true;
                }
            }

            internal void IncreaseMeleeDamage(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(meleeDamageBonus);
                    description = "Melee damage +" + Math.Round(value * 100, 1) + "%";

                    isInitialized = true;
                }


                if (enabled)
                {
                    PlayerStats.Instance.MeleeDamageFactor += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.MeleeDamageFactor -= value;
                    isReady = true;
                }
            }

            // potential bug
            internal void IncreaseMeleeSpeed(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(meleeSpeedBonus);
                    description = "Melee speed +" + Math.Round(value * 100, 1) + "%";

                    isInitialized = true;
                }


                if (enabled)
                {
                    PlayerStats.Instance.MeleeSpeedFactor += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.MeleeSpeedFactor -= value;
                    isReady = true;
                }
            }

            internal void IncreaseMoveSpeed(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(moveSpeedBonus);
                    description = "Move speed +" + Math.Round(value * 100, 1) + "%";

                    isInitialized = true;
                }


                if (enabled)
                {
                    PlayerStats.Instance.MoveSpeedFactor += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.MoveSpeedFactor -= value;
                    isReady = true;
                }
            }

            internal void IncreaseExperience(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(experienceBonus);
                    description = "Experience gain +" + Math.Round(value * 100, 1) + "%";

                    isInitialized = true;
                }


                if (enabled)
                {
                    PlayerStats.Instance.ExperienceMultiplier += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.ExperienceMultiplier -= value;
                    isReady = true;
                }
            }






            public string Description { get { return description; } }
        } 
    }
}
