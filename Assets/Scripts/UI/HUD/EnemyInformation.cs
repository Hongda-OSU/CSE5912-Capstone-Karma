using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class EnemyInformation : UI
    {
        [SerializeField] private Camera currentCamera;

        [SerializeField] private float widthPerUnit = 1f;

        [SerializeField] private float distanceToDisplay = 10f;

        private VisualElement healthBar;
        private VisualElement maxHealthBar;

        private float prevHealth;

        private GameObject target;
        private GameObject pivot;
        private Enemy enemy;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            Initialize();

            maxHealthBar = root.Q<VisualElement>("MaxHealthBar");
            healthBar = maxHealthBar.Q<VisualElement>("HealthBar");

            target = transform.parent.gameObject;
            pivot = gameObject;
            enemy = target.GetComponent<Enemy>();
        }


        private void LateUpdate()
        {
            currentCamera = WeaponManager.Instance.CarriedWeapon.GunCamera;

            if (DisplayEnabled())
            {
                SetHealthBar();
                SetPosition();
            }
            else
            {
                maxHealthBar.style.display = DisplayStyle.None;
            }
        }

        private bool DisplayEnabled()
        {
            float distance = Vector3.Distance(PlayerManager.Instance.Player.transform.position, transform.position);

            GameObject player = PlayerManager.Instance.Player;
            RaycastHit hit;

            if (distance < distanceToDisplay)
            {
                if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, distanceToDisplay))
                {
                    return hit.transform.gameObject == player;
                }
            }
            return false;
        }
        private void SetHealthBar()
        {

            var width = widthPerUnit * enemy.MaxHealth;

            var planes = GeometryUtility.CalculateFrustumPlanes(currentCamera);
            if (enemy.Health > 0 && GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Collider>().bounds))
            {
                healthBar.style.display = DisplayStyle.Flex;
                healthBar.style.width = width * enemy.Health / enemy.MaxHealth;

                maxHealthBar.style.display = DisplayStyle.Flex;
                maxHealthBar.style.width = width;

                var deltaHealth = prevHealth - enemy.Health;
                if (deltaHealth > 0)
                {
                    VisualElement deltaEffect = new VisualElement();
                    maxHealthBar.Add(deltaEffect);

                    deltaEffect.style.width = deltaHealth * widthPerUnit;
                    deltaEffect.style.height = healthBar.resolvedStyle.height;
                    deltaEffect.style.backgroundColor = Color.white;
                    deltaEffect.style.left = enemy.Health * widthPerUnit;
                    deltaEffect.style.position = Position.Absolute;

                    StartCoroutine(FadeOut(deltaEffect));
                }


                prevHealth = enemy.Health;
            }
            else
            {
                healthBar.style.display = DisplayStyle.None;
                maxHealthBar.style.display = DisplayStyle.None;
            }
        }
        private void SetPosition()
        {
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(maxHealthBar.panel, pivot.transform.position, currentCamera);

            root.transform.position = new Vector2(newPosition.x - widthPerUnit * enemy.MaxHealth / 2, newPosition.y);
        }

        protected override IEnumerator FadeOut(VisualElement element)
        {
            yield return base.FadeOut(element);
            maxHealthBar.Remove(element);
        }
    }
}
