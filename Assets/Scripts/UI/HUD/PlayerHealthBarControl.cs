
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class PlayerHealthBarControl : UI
    {
        [SerializeField] private float lengthPerUnit = 2f;

        private VisualElement healthBar;
        private VisualElement maxHealthBar;

        private VisualElement energyBar;
        private VisualElement maxEnergyBar;

        private VisualElement armorBar;
        private VisualElement maxArmorBar;


        private static PlayerHealthBarControl instance;
        public static PlayerHealthBarControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            Initialize();

            maxHealthBar = root.Q<VisualElement>("MaxHealthBar");
            healthBar = maxHealthBar.Q<VisualElement>("Current");

            maxEnergyBar = root.Q<VisualElement>("MaxEnergyBar");
            energyBar = maxEnergyBar.Q<VisualElement>("Current");

            maxArmorBar = root.Q<VisualElement>("MaxArmorBar");
            armorBar = maxArmorBar.Q<VisualElement>("Current");
        }

        private void Update()
        {
            UpdateBars();
        }

        public void UpdateBars()
        {
            healthBar.style.width = PlayerStats.Instance.Health * lengthPerUnit;
            maxHealthBar.style.width = PlayerStats.Instance.MaxHealth * lengthPerUnit;

            energyBar.style.width = PlayerStats.Instance.Shield_energy * lengthPerUnit;
            maxEnergyBar.style.width = PlayerStats.Instance.MaxShield_energy * lengthPerUnit;

            armorBar.style.width = PlayerStats.Instance.Shield_armor * lengthPerUnit;
            maxArmorBar.style.width = PlayerStats.Instance.MaxShield_armor * lengthPerUnit;
        }

        public override void Display(bool enabled)
        {
            if (enabled)
            {
                healthBar.style.display = DisplayStyle.Flex;
                maxHealthBar.style.display = DisplayStyle.Flex;
            }
            else
            {
                healthBar.style.display = DisplayStyle.None;
                maxHealthBar.style.display = DisplayStyle.None;
            }
        }
    }
}