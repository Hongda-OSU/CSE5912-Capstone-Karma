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

        private void Initialize()
        {
            var type = attachment.Type;
            // to-do
            attachment.AttachmentName = attachment.Rarity.ToString() + type;

            bonus = new Bonus(attachment);

            var list = bonus.typeToFunctionList[type];
            bonus.AssignBonusFunction(type, UnityEngine.Random.Range(0, list.Count));

        }

        public void GetRuneBonus(int index)
        {
            bonus = new Bonus(attachment);

            bonus.AssignBonusFunction(index);
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
                    HighDamageLowResist,
                    HighSpeedLowDamage,
                    HighCritChanceLowCritDamage,
                    HealthToShield,
                    Tank,
                    Vampire,
                    MeleeAndBullet,

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

            internal void AssignBonusFunction(int index)
            {
                var func = typeToFunctionList[Attachment.AttachmentType.Rune][index];

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

                int ammo = WeaponManager.Instance.CarriedWeapon.MaxAmmoPerMag;
                if (enabled)
                {
                    WeaponManager.Instance.CarriedWeapon.MaxAmmoPerMag = (int)Mathf.Floor(ammo * (1 + value));
                    isReady = false;
                }
                else
                {
                    WeaponManager.Instance.CarriedWeapon.MaxAmmoPerMag = (int)Mathf.Ceil(ammo / (1 + value));
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
                    PlayerStats.Instance.ReloadSpeedFactor += value;
                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.ReloadSpeedFactor -= value;
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
                    description = "Spread -" + Math.Round(value * 100, 1) + "%";
                    value *= WeaponManager.Instance.CarriedWeapon.SpreadAngle;

                    isInitialized = true;
                }


                if (enabled)
                {
                    WeaponManager.Instance.CarriedWeapon.SpreadAngle -= value;
                    isReady = false;
                }
                else
                {
                    WeaponManager.Instance.CarriedWeapon.SpreadAngle += value;
                    isReady = true;
                }
            }


            internal void DecreaseRecoil(bool enabled)
            {
                if (!isInitialized)
                {
                    value = ResolveValue(recoilReductionBonus);
                    description = "Recoil -" + Math.Round(value * 100, 1) + "%";
                    value *= FPSMouseLook.Instance.RecoilScale;

                    isInitialized = true;
                }


                if (enabled)
                {
                    FPSMouseLook.Instance.RecoilScale -= value;
                    isReady = false;
                }
                else
                {
                    FPSMouseLook.Instance.RecoilScale += value;
                    isReady = true;
                }
            }


            /*
             *  Rune
             */

            internal void HighDamageLowResist(bool enabled)
            {
                float damage = 1f;
                float resist = 200f;

                description = 
                    "All damages +" + damage * 100 + "%" +
                    "\nAll resists -" + resist;

                if (enabled)
                {
                    PlayerStats.Instance.GetDamageFactor().Physical.Value += damage;
                    PlayerStats.Instance.GetDamageFactor().Fire.Value += damage;
                    PlayerStats.Instance.GetDamageFactor().Cryo.Value += damage;
                    PlayerStats.Instance.GetDamageFactor().Electro.Value += damage;
                    PlayerStats.Instance.GetDamageFactor().Venom.Value += damage;

                    PlayerStats.Instance.GetResist().Physical.Value += resist;
                    PlayerStats.Instance.GetResist().Fire.Value += resist;
                    PlayerStats.Instance.GetResist().Cryo.Value += resist;
                    PlayerStats.Instance.GetResist().Electro.Value += resist;
                    PlayerStats.Instance.GetResist().Venom.Value += resist;

                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetDamageFactor().Physical.Value -= damage;
                    PlayerStats.Instance.GetDamageFactor().Fire.Value -= damage;
                    PlayerStats.Instance.GetDamageFactor().Cryo.Value -= damage;
                    PlayerStats.Instance.GetDamageFactor().Electro.Value -= damage;
                    PlayerStats.Instance.GetDamageFactor().Venom.Value -= damage;

                    PlayerStats.Instance.GetResist().Physical.Value -= resist;
                    PlayerStats.Instance.GetResist().Fire.Value -= resist;
                    PlayerStats.Instance.GetResist().Cryo.Value -= resist;
                    PlayerStats.Instance.GetResist().Electro.Value -= resist;
                    PlayerStats.Instance.GetResist().Venom.Value -= resist;
                    isReady = true;
                }
            }

            internal void HighSpeedLowDamage(bool enabled)
            {
                float speed = 1f;
                float damage = 0.5f;

                description =
                    "Speed +" + speed * 100 + "%" +
                    "\nAll damages -" + damage * 100 + "%";

                if (enabled)
                {
                    PlayerStats.Instance.MoveSpeedFactor += speed;

                    PlayerStats.Instance.GetDamageFactor().Physical.Value -= damage;
                    PlayerStats.Instance.GetDamageFactor().Fire.Value -= damage;
                    PlayerStats.Instance.GetDamageFactor().Cryo.Value -= damage;
                    PlayerStats.Instance.GetDamageFactor().Electro.Value -= damage;
                    PlayerStats.Instance.GetDamageFactor().Venom.Value -= damage;

                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.MoveSpeedFactor -= speed;

                    PlayerStats.Instance.GetDamageFactor().Physical.Value += damage;
                    PlayerStats.Instance.GetDamageFactor().Fire.Value += damage;
                    PlayerStats.Instance.GetDamageFactor().Cryo.Value += damage;
                    PlayerStats.Instance.GetDamageFactor().Electro.Value += damage;
                    PlayerStats.Instance.GetDamageFactor().Venom.Value += damage;

                    isReady = true;
                }
            }

            internal void HighCritChanceLowCritDamage(bool enabled)
            {
                float chance = 0.5f;
                float damage = 0.5f;

                description =
                    "Crit chance +" + chance * 100 + "%" + 
                    "\nCrit damage -" + damage * 100 + "%";

                if (enabled)
                {
                    PlayerStats.Instance.CritRate += chance;
                    PlayerStats.Instance.CritDamageFactor -= damage;

                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.CritRate -= chance;
                    PlayerStats.Instance.CritDamageFactor += damage;

                    isReady = true;
                }
            }

            internal void HealthToShield(bool enabled)
            {
                description =
                    "Convert health to energy shield and armor shield";

                if (enabled)
                {
                    PlayerSkillManager.Instance.GetBonusSkill("HealthToShield").LevelUp();

                    isReady = false;
                }
                else
                {
                    PlayerSkillManager.Instance.GetBonusSkill("HealthToShield").ResetLevel();

                    isReady = true;
                }
            }


            internal void Tank(bool enabled)
            {
                float resist = 500f;
                float slow = 0.75f;

                description =
                    "All resists +" + resist +
                    "\nMove speed -" + slow * 100 + "%";

                if (enabled)
                {
                    PlayerStats.Instance.GetResist().Physical.Value += resist;
                    PlayerStats.Instance.GetResist().Fire.Value += resist;
                    PlayerStats.Instance.GetResist().Cryo.Value += resist;
                    PlayerStats.Instance.GetResist().Electro.Value += resist;
                    PlayerStats.Instance.GetResist().Venom.Value += resist;

                    PlayerStats.Instance.MoveSpeedFactor -= slow;

                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.GetResist().Physical.Value -= resist;
                    PlayerStats.Instance.GetResist().Fire.Value -= resist;
                    PlayerStats.Instance.GetResist().Cryo.Value -= resist;
                    PlayerStats.Instance.GetResist().Electro.Value -= resist;
                    PlayerStats.Instance.GetResist().Venom.Value -= resist;

                    PlayerStats.Instance.MoveSpeedFactor += slow;

                    isReady = true;
                }
            }

            internal void Vampire(bool enabled)
            {
                float vamp = 0.5f;
                float takeDamage = 1f;

                description =
                    "Heal for " + vamp * 100 + "% of damage dealt by firearms" +
                    "\nTake double damage";

                if (enabled)
                {
                    PlayerStats.Instance.BulletVamp += vamp;

                    PlayerStats.Instance.TakeDamageFactor += takeDamage;

                    isReady = false;
                }
                else
                {
                    PlayerStats.Instance.BulletVamp -= vamp;

                    PlayerStats.Instance.TakeDamageFactor -= takeDamage;

                    isReady = true;
                }
            }

            internal void MeleeAndBullet(bool enabled)
            {
                description =
                    "Exchange the damages and elements of firearm and melee attack";

                if (enabled)
                {
                    var damage = WeaponManager.Instance.CarriedWeapon.Damage;
                    var element = WeaponManager.Instance.CarriedWeapon.Element;

                    WeaponManager.Instance.CarriedWeapon.Damage = MeleeAttack.Instance.BaseDamage;
                    WeaponManager.Instance.CarriedWeapon.Element = MeleeAttack.Instance.ElementType;

                    MeleeAttack.Instance.BaseDamage = damage;
                    MeleeAttack.Instance.ElementType = element;

                    isReady = false;
                }
                else
                {
                    var damage = WeaponManager.Instance.CarriedWeapon.Damage;
                    var element = WeaponManager.Instance.CarriedWeapon.Element;

                    WeaponManager.Instance.CarriedWeapon.Damage = MeleeAttack.Instance.BaseDamage;
                    WeaponManager.Instance.CarriedWeapon.Element = MeleeAttack.Instance.ElementType;

                    MeleeAttack.Instance.BaseDamage = damage;
                    MeleeAttack.Instance.ElementType = element;

                    isReady = true;
                }
            }

            public string Description { get { return description; } }
        } 
    }
}
