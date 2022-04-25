
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
                return;
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
            maxHealthBar.style.width = PlayerStats.Instance.MaxHealth * lengthPerUnit;
            healthBar.style.width = PlayerStats.Instance.Health / PlayerStats.Instance.MaxHealth * maxHealthBar.resolvedStyle.width;

            maxEnergyBar.style.width = PlayerStats.Instance.MaxShield_energy * lengthPerUnit;
            energyBar.style.width = PlayerStats.Instance.Shield_energy / PlayerStats.Instance.MaxShield_energy * maxEnergyBar.resolvedStyle.width;

            maxArmorBar.style.width = PlayerStats.Instance.MaxShield_armor * lengthPerUnit;
            armorBar.style.width = PlayerStats.Instance.Shield_armor / PlayerStats.Instance.MaxShield_armor * maxArmorBar.resolvedStyle.width;
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