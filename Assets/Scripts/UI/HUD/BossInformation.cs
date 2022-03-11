using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class BossInformation : UI
    {
        [SerializeField] private float distanceToDisplay = 10f;

        [SerializeField] private float width = 1000f;
        [SerializeField] private float height = 20f;

        private Label detail;
        private VisualElement healthBar;
        private VisualElement maxHealthBar;
        private bool displayHealthBar = false;

        private float prevHealth;

        private GameObject target;
        private Enemy enemy;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            Initialize();

            detail = root.Q<Label>("Detail");
            maxHealthBar = root.Q<VisualElement>("MaxHealthBar");
            healthBar = maxHealthBar.Q<VisualElement>("HealthBar");

            target = transform.parent.gameObject;
            enemy = target.GetComponent<Enemy>();
        }

        public void DisplayHealthBar(bool enabled)
        {
            displayHealthBar = enabled;
        }

        private void LateUpdate()
        {
            if (displayHealthBar)
            {
                //StartCoroutine(TriggerEffect());
                SetHealthBar();
            }
            else
            {
                maxHealthBar.style.width = 0f;
                healthBar.style.width = 0f;
                root.style.display = DisplayStyle.None;
            }
        }

        private bool DisplayEnabled()
        {
            float maxDistance = distanceToDisplay;
            //if (enemy.IsAttackedByPlayer)
            //    maxDistance = distanceToDisplayIfAttacked;

            float distance = Vector3.Distance(PlayerManager.Instance.Player.transform.position, transform.position);

            if (distance < maxDistance)
            {
                return true;
            }

            return false;
        }
        private void SetHealthBar()
        {
            root.style.display = DisplayStyle.Flex;

            detail.text = enemy.EnemyName;

            if (enemy.Health > 0)
            {
                healthBar.style.display = DisplayStyle.Flex;
                healthBar.style.width = width * enemy.Health / enemy.MaxHealth;

                maxHealthBar.style.display = DisplayStyle.Flex;
                maxHealthBar.style.width = width;

                var deltaHealth = prevHealth - enemy.Health;
                if (deltaHealth > 0)
                {
                    healthBar.style.transitionDuration = new StyleList<TimeValue>(0f);
                    maxHealthBar.style.transitionDuration = new StyleList<TimeValue>(0f);

                    VisualElement deltaEffect_right = new VisualElement();
                    maxHealthBar.Add(deltaEffect_right);

                    deltaEffect_right.style.width = width * deltaHealth / enemy.MaxHealth / 2;
                    deltaEffect_right.style.height = healthBar.resolvedStyle.height;
                    deltaEffect_right.style.backgroundColor = Color.white;
                    deltaEffect_right.style.left = width / 2 + width * enemy.Health / enemy.MaxHealth / 2;
                    deltaEffect_right.style.position = Position.Absolute;

                    StartCoroutine(FadeOut(deltaEffect_right));


                    VisualElement deltaEffect_left = new VisualElement();
                    maxHealthBar.Add(deltaEffect_left);

                    deltaEffect_left.style.width = width * deltaHealth / enemy.MaxHealth / 2;
                    deltaEffect_left.style.height = healthBar.resolvedStyle.height;
                    deltaEffect_left.style.backgroundColor = Color.white;
                    deltaEffect_left.style.right = width / 2 + width * enemy.Health / enemy.MaxHealth / 2;
                    deltaEffect_left.style.position = Position.Absolute;

                    StartCoroutine(FadeOut(deltaEffect_left));
                }


                prevHealth = enemy.Health;
            }
        }
    }
}
