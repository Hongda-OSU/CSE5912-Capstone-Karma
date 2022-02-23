using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class AttachmentBonus
    {
        private Attachment attachment;

        private List<int> availableBonusIndex;
        private List<Bonus> bonusList;
        public AttachmentBonus(Attachment attachment)
        {
            this.attachment = attachment; 
            Initialize();
        }

        public void Initialize()
        {
            attachment.name = attachment.Rarity.ToString() + attachment.Type;

            bonusList = new List<Bonus>();
            for (int i = 0; i < (int)attachment.Rarity + 1; i++)
            {
                Bonus bonus = new Bonus(attachment);
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

            private Attachment attachment;

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
            private static float recoilReductionBonus = 0.15f;
            private static float spreadReductionBonus = 0.15f;
            private static float fireRateBonus = 0.05f;

            //internal enum 
            internal Bonus(Attachment attachment)
            {
                this.attachment = attachment;
                level = (int)attachment.Rarity + 1;
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
                    DecreaseRecoil,
                    DecreaseSpread,
                    IncreaseFireRate,
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


            internal void IncreaseCritDamage(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(critDamageBonus);
                    isInitialized = true;
                }

                description = "Critical Damage +" + Math.Round(value * 100, 1) + "%";

                if (enabled)
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

            internal void IncreaseCritRate(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(critRateBonus);
                    isInitialized = true;
                }

                description = "Critical Rate +" + Math.Round(value * 100, 1) + "%";

                if (enabled)
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

            internal void IncreaseAmmo(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(ammoBonus);
                    isInitialized = true;
                }

                description = "Ammo +" + Math.Round(value * 100, 1) + "%";

                int ammo = WeaponManager.Instance.CarriedWeapon.AmmoInMag;
                if (enabled)
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

            internal void DecreaseRecoil(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(recoilReductionBonus);
                    isInitialized = true;
                }

                description = "Recoil -" + Math.Round(value * 100, 1) + "%";

                if (enabled)
                {
                    FPSMouseLook.Instance.RecoilScale *= 1 - value;
                    isReady = false;
                }
                else
                {
                    FPSMouseLook.Instance.RecoilScale /= 1 - value;
                    isReady = true;
                }
            }

            internal void DecreaseSpread(bool enabled)
            {
                if (enabled != isReady)
                    return;

                if (!isInitialized)
                {
                    value = ResolveValue(spreadReductionBonus);
                    isInitialized = true;
                }

                description = "Spread -" + Math.Round(value * 100, 1) + "%";

                if (enabled)
                {
                    WeaponManager.Instance.CarriedWeapon.SpreadAngle *= (1 - value);
                    isReady = false;
                }
                else
                {
                    WeaponManager.Instance.CarriedWeapon.SpreadAngle /= (1 - value);
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
                    WeaponManager.Instance.CarriedWeapon.FireRate *= (1 + value);
                    isReady = false;
                }
                else
                {
                    WeaponManager.Instance.CarriedWeapon.FireRate /= (1 + value);
                    isReady = true;
                }
            }

            public string Description { get { return description; } }
        } 
    }
}
