
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class PlayerHealthBarControl : UI
    {
        [SerializeField] private float lengthPerUnit = 2f;

        private VisualElement healthBar;
        private VisualElement maxHealthBar;


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

            healthBar = root.Q<VisualElement>("HealthBar");
            maxHealthBar = root.Q<VisualElement>("MaxHealthBar");
        }

        private void Start()
        {
            UpdateHealthBar();
        }

        public void UpdateHealthBar()
        {
            healthBar.style.width = PlayerStats.Instance.Health * lengthPerUnit;
            maxHealthBar.style.width = PlayerStats.Instance.MaxHealth * lengthPerUnit;
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