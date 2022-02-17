using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class EnemyInformation : UI
    {
        [SerializeField] private Camera playerCamera;

        [SerializeField] private float widthPerUnit = 1f;

        private VisualElement healthBar;
        private VisualElement maxHealthBar;

        private float prevHealth;

        private GameObject target;
        private GameObject pivot;
        private IEnemy enemy;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            Initialize();

            maxHealthBar = root.Q<VisualElement>("MaxHealthBar");
            healthBar = maxHealthBar.Q<VisualElement>("HealthBar");

            target = transform.parent.gameObject;
            pivot = gameObject;
            enemy = target.GetComponent<IEnemy>();
        }

        private void Start()
        {
            playerCamera = PlayerManager.Instance.PlayerCamera;
        }

        private void LateUpdate()
        {
            SetHealthBar();

            SetPosition();
        }

        private void SetHealthBar()
        {
            var width = widthPerUnit * enemy.GetMaxHealth();

            var planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
            if (enemy.GetHealth() > 0 && GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Collider>().bounds))
            {
                healthBar.style.display = DisplayStyle.Flex;
                healthBar.style.width = width * enemy.GetHealth() / enemy.GetMaxHealth();

                maxHealthBar.style.display = DisplayStyle.Flex;
                maxHealthBar.style.width = width;

                var deltaHealth = prevHealth - enemy.GetHealth();
                if (deltaHealth > 0)
                {
                    VisualElement deltaEffect = new VisualElement();
                    maxHealthBar.Add(deltaEffect);

                    deltaEffect.style.width = deltaHealth * widthPerUnit;
                    deltaEffect.style.height = healthBar.resolvedStyle.height;
                    deltaEffect.style.backgroundColor = Color.white;
                    deltaEffect.style.left = enemy.GetHealth() * widthPerUnit;
                    deltaEffect.style.position = Position.Absolute;

                    StartCoroutine(FadeOut(deltaEffect));
                }


                prevHealth = enemy.GetHealth();
            }
            else
            {
                healthBar.style.display = DisplayStyle.None;
                maxHealthBar.style.display = DisplayStyle.None;
            }
        }
        private void SetPosition()
        {
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(maxHealthBar.panel, pivot.transform.position, playerCamera);

            root.transform.position = new Vector2(newPosition.x - widthPerUnit * enemy.GetMaxHealth() / 2, newPosition.y);
        }

        protected override IEnumerator FadeOut(VisualElement element)
        {
            yield return base.FadeOut(element);
            maxHealthBar.Remove(element);
        }
    }
}
