using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerSkillManager : MonoBehaviour
    {

        [SerializeField] private int skillPoints = 0;
        [SerializeField] private GameObject skillTree_element;

        [SerializeField] private GameObject attachmentSetSkills;
        private Dictionary<Attachment.AttachmentSet, PlayerSkill> setToSkill = new Dictionary<Attachment.AttachmentSet, PlayerSkill>();

        [SerializeField] private GameObject bonusSkills;
        private Dictionary<string, PlayerSkill> bonusToSkill = new Dictionary<string, PlayerSkill>();


        private PlayerSkill mainSkill;
        private PlayerSkill setSkill;


        private static PlayerSkillManager instance;
        public static PlayerSkillManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

        }

        private void Start()
        {
            SkillsPanelControl.Instance.SkillTree_element.AssignMain(skillTree_element.transform.GetComponentInChildren<Epiphany>());

            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(0, skillTree_element.transform.GetComponentInChildren<FireAscension>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(1, skillTree_element.transform.GetComponentInChildren<CryoAscension>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(2, skillTree_element.transform.GetComponentInChildren<ElectroAscension>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(3, skillTree_element.transform.GetComponentInChildren<VenomAscension>());

            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(4, skillTree_element.transform.GetComponentInChildren<FireMastery>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(5, skillTree_element.transform.GetComponentInChildren<CryoMastery>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(6, skillTree_element.transform.GetComponentInChildren<ElectroMastery>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(7, skillTree_element.transform.GetComponentInChildren<VenomMastery>());

            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(8, skillTree_element.transform.GetComponentInChildren<FireIntellection>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(9, skillTree_element.transform.GetComponentInChildren<CryoIntellection>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(10, skillTree_element.transform.GetComponentInChildren<ElectroIntellection>());
            SkillsPanelControl.Instance.SkillTree_element.AssignBuff(11, skillTree_element.transform.GetComponentInChildren<VenomIntellection>());

            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(0, skillTree_element.GetComponentInChildren<Incendiary>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(1, skillTree_element.GetComponentInChildren<Inferno>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(2, skillTree_element.GetComponentInChildren<Ignite>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(3, skillTree_element.GetComponentInChildren<LivingFlame>());

            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(4, skillTree_element.GetComponentInChildren<Everfrost>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(5, skillTree_element.GetComponentInChildren<Iceborn>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(6, skillTree_element.GetComponentInChildren<Frostbite>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(7, skillTree_element.GetComponentInChildren<Eternity>());

            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(8, skillTree_element.GetComponentInChildren<LightningBolt>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(9, skillTree_element.GetComponentInChildren<LightningChain>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(10, skillTree_element.GetComponentInChildren<Detonation>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(11, skillTree_element.GetComponentInChildren<Smite>());

            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(12, skillTree_element.GetComponentInChildren<Pandemic>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(13, skillTree_element.GetComponentInChildren<Fester>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(14, skillTree_element.GetComponentInChildren<Perish>());
            SkillsPanelControl.Instance.SkillTree_element.AssignPassive(15, skillTree_element.GetComponentInChildren<MixedInfection>());


            setToSkill.Add(Attachment.AttachmentSet.AtomBreak, attachmentSetSkills.GetComponentInChildren<AtomBreak>());
            setToSkill.Add(Attachment.AttachmentSet.VoidKnight, attachmentSetSkills.GetComponentInChildren<VoidKnight>());
            setToSkill.Add(Attachment.AttachmentSet.Sergeant76, attachmentSetSkills.GetComponentInChildren<Sergeant76>());
            setToSkill.Add(Attachment.AttachmentSet.BlackSoul, attachmentSetSkills.GetComponentInChildren<BlackSoul>());
            setToSkill.Add(Attachment.AttachmentSet.Leviathan, attachmentSetSkills.GetComponentInChildren<Leviathan>());


            bonusToSkill.Add("HealthToShield", bonusSkills.GetComponentInChildren<HealthToShield>());
        }

        public void SetMainSkill(PlayerSkill skill)
        {
            if (skill.Type != PlayerSkill.SkillType.Main)
                Debug.LogError("Error: skill type is not Main. ");

            mainSkill = skill;
            mainSkill.TimeSincePerformed = mainSkill.Cooldown;
            SkillInformationControl.Instance.SetupMainSkill(mainSkill);
        }

        public PlayerSkill GetSetSkill(Attachment.AttachmentSet set)
        {
            return setToSkill[set];
        }
        public PlayerSkill GetBonusSkill(string name)
        {
            return bonusToSkill[name];
        }

        public void TryActivateSetSkill()
        {
            var weapon = WeaponManager.Instance.CarriedWeapon;
            if (IsSetSkillActivatable(weapon))
            {
                var set = weapon.Attachments[0].Set;
                foreach (var kvp in setToSkill)
                {
                    if (kvp.Key == set)
                    {
                        kvp.Value.LevelUp();
                        setSkill = kvp.Value;
                    }
                    else
                    {
                        kvp.Value.ResetLevel();
                    }
                }
            }
            else
            {
                setSkill = null;
                foreach (var kvp in setToSkill)
                {
                    kvp.Value.ResetLevel();
                }
            }
            SkillInformationControl.Instance.SetupSetSkill(setSkill);
        }
        public bool IsSetSkillActivatable(Firearms weapon)
        {
            Attachment.AttachmentSet set = Attachment.AttachmentSet.AtomBreak;

            // the last attachment is rune.
            // only divine attachments trigger set skills
            for (int i = 0; i < weapon.Attachments.Length - 1; i++)
            {
                var attachment = weapon.Attachments[i];
                if (attachment == null || attachment.Rarity != Attachment.AttachmentRarity.Divine)
                    return false;
                if (i == 0)
                    set = attachment.Set;

                if (attachment.Set != set)
                    return false;
            }
            return true;
        }



        public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }
        public PlayerSkill MainSkill { get { return mainSkill; } }
    }
}
