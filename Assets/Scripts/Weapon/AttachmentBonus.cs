using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class AttachmentBonus
    {
        private Attachment attachment;

        private Bonus bonus;

        public AttachmentBonus(Attachment attachment)
        {
            this.attachment = attachment; 
            Initialize();
        }

        public void Initialize()
        {
            var type = attachment.Type;
            // to-do
            attachment.AttachmentName = attachment.Rarity.ToString() + type;

            bonus = new Bonus(attachment);

            var list = bonus.typeToFunctionList[type];
            bonus.AssignBonusFunction(type, UnityEngine.Random.Range(0, list.Count));

        }

        public void Perform(bool enabled)
        {
            bonus.Perform(enabled);
        }

        public List<string> GetBonusDescriptionList()
        {
            var list = new List<string>();
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

            internal BonusFunction bonusFunction;

            internal Dictionary<Attachment.AttachmentType, List<BonusFunction>> typeToFunctionList;

            internal static List<BonusFunction> bulletBonusList;
            internal static List<BonusFunction> scopeBonusList;
            internal static List<BonusFunction> casingBonusList;
            internal static List<BonusFunction> runeBonusList;

            private static float factorPerLevel = 0.5f;
            private static float valueVariance = 0.3f;

            private static float critDamageBonus = 0.06f;
            private static float critRateBonus = 0.12f;

            private static float recoilReductionBonus = 0.15f;
            private static float spreadReductionBonus = 0.15f;

            private static float ammoBonus = 0.2f;
            private static float reloadSpeedBonus = 0.1f;

            //internal enum 
            internal Bonus(Attachment attachment)
            {
                this.attachment = attachment;
                level = (int)attachment.Rarity + 1;
                isReady = true;

                bulletBonusList = new List<BonusFunction>()
                {
                    IncreaseCritDamage,
                    IncreaseCritRate,
                };

                scopeBonusList = new List<BonusFunction>()
                {
                    DecreaseRecoil,
                    DecreaseSpread,
                };

                casingBonusList = new List<BonusFunction>()
                {
                    IncreaseAmmo,
                    IncreaseReloadSpeed,
                };

                runeBonusList = new List<BonusFunction>()
                {
                    ExtremeDamage_fire,

                };

                

                typeToFunctionList = new Dictionary<Attachment.AttachmentType, List<BonusFunction>>();
                typeToFunctionList.Add(Attachment.AttachmentType.Bullet, bulletBonusList);
                typeToFunctionList.Add(Attachment.AttachmentType.Scope, scopeBonusList);
                typeToFunctionList.Add(Attachment.AttachmentType.Casing, casingBonusList);
                typeToFunctionList.Add(Attachment.AttachmentType.Rune, runeBonusList);
            }

            internal void AssignBonusFunction(Attachment.AttachmentType type, int index)
            {
                var func = typeToFunctionList[type][index];

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
             *  Bullet
             */

            internal void IncreaseCritDamage(bool enabled)
            {
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


            /*
             *  Casing
             */

            internal void IncreaseAmmo(bool enabled)
            {
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

            internal void IncreaseReloadSpeed(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(reloadSpeedBonus);
                    isInitialized = true;
                }

                description = "Reload Speed +" + Math.Round(value * 100, 1) + "%";

                if (enabled)
                {
                    WeaponManager.Instance.CarriedWeapon.ReloadSpeed *= 1 + value;
                    isReady = false;
                }
                else
                {
                    WeaponManager.Instance.CarriedWeapon.ReloadSpeed /= 1 + value;
                    isReady = true;
                }
            }


            /*
             *  Scope
             */

            internal void DecreaseSpread(bool enabled)
            {
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


            internal void DecreaseRecoil(bool enabled)
            {
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


            /*
             *  Rune
             */

            internal void ExtremeDamage_fire(bool enabled)
            {
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

            public string Description { get { return description; } }
        } 
    }
}
