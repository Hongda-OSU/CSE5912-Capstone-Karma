using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class EnemyHealthBar : UI
    {
        [SerializeField] private Camera currentCamera;

        [SerializeField] private float width = 200f;

        [SerializeField] private float distanceToDisplay = 10f;
        [SerializeField] private float distanceToDisplayIfAttacked = 20f;

        private VisualElement healthBar;
        private VisualElement maxHealthBar;

        private VisualElement debuffs;
        private List<DebuffSlot> debuffSlotList;

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

            debuffs = root.Q<VisualElement>("Debuffs");
            debuffSlotList = new List<DebuffSlot>();
            foreach (var child in debuffs.Children())
            {
                var slot = new DebuffSlot(child);
                debuffSlotList.Add(slot);
                slot.Clear();
            }

            target = transform.parent.gameObject;
            pivot = gameObject;
            enemy = target.GetComponent<Enemy>();
        }


        private void LateUpdate()
        {
            currentCamera = WeaponManager.Instance.CarriedWeapon.GunCamera;

            if (DisplayEnabled() && enemy.IsAlive)
            {
                SetHealthBar();
                SetDebuffs();
                SetPosition();
            }
            else
            {
                maxHealthBar.style.display = DisplayStyle.None;
                debuffs.style.display = DisplayStyle.None;
            }
        }

        private bool DisplayEnabled()
        {
            float maxDistance = distanceToDisplay;
            if (enemy.IsAttackedByPlayer)
                maxDistance = distanceToDisplayIfAttacked;

            float distance = enemy.DistanceToPlayer;
            GameObject player = PlayerManager.Instance.Player;
            RaycastHit hit;

            if (distance < maxDistance)
            {
                if (Physics.Raycast(target.transform.position, enemy.DirectionToPlayer, out hit, maxDistance))
                {
                    return hit.transform.gameObject == player;
                }
            }
            return false;
        }
        private void SetHealthBar()
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(currentCamera);

            if (GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Collider>().bounds))
            {
                healthBar.style.display = DisplayStyle.Flex;
                healthBar.style.width = width * enemy.Health / enemy.MaxHealth;

                maxHealthBar.style.display = DisplayStyle.Flex;
                maxHealthBar.style.width = width;

                var deltaHealth = prevHealth - enemy.Health;

                DamagedEffect(deltaHealth);

                prevHealth = enemy.Health;
            }
            else
            {
                healthBar.style.display = DisplayStyle.None;
                maxHealthBar.style.display = DisplayStyle.None;
            }
        }

        private void DamagedEffect(float deltaHealth)
        {
            if (deltaHealth > 0)
            {
                VisualElement deltaEffect = new VisualElement();
                maxHealthBar.Add(deltaEffect);

                deltaEffect.style.width = deltaHealth / enemy.MaxHealth * width;
                deltaEffect.style.height = healthBar.resolvedStyle.height;
                deltaEffect.style.backgroundColor = Color.white;
                deltaEffect.style.left = enemy.Health / enemy.MaxHealth * width;
                deltaEffect.style.position = Position.Absolute;

                StartCoroutine(FadeOut(deltaEffect));
            }
        }

        private void SetDebuffs()
        {
            debuffs.style.display = DisplayStyle.Flex;

            debuffSlotList[0].Display(enemy.Burned);
            debuffSlotList[1].Display(enemy.Frozen);
            debuffSlotList[2].Display(enemy.Electrocuted);
            debuffSlotList[3].Display(enemy.Infected);
        }

        private void SetPosition()
        {
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(maxHealthBar.panel, pivot.transform.position, currentCamera);

            root.transform.position = new Vector2(
                newPosition.x - root.Q<VisualElement>("Root").resolvedStyle.width / 2, 
                newPosition.y - root.Q<VisualElement>("Root").resolvedStyle.height);
        }

        protected override IEnumerator FadeOut(VisualElement element)
        {
            yield return base.FadeOut(element);
            maxHealthBar.Remove(element);
        }


        internal class DebuffSlot
        {
            private VisualElement slot;
            private Label stack;

            private Debuff debuff;

            public DebuffSlot(VisualElement slot)
            {
                this.slot = slot;
                stack = slot.Q<Label>("Stack");
                slot.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            }

            public void Display(Debuff debuff)
            {
                if (debuff.Stack == 0)
                {
                    Clear();
                    return;
                }

                slot.style.display = DisplayStyle.Flex;
                
                stack.text = debuff.Stack.ToString();

                slot.style.backgroundImage = new StyleBackground(debuff.Icon);
            }

            public void Clear()
            {
                slot.style.display = DisplayStyle.None;
                debuff = null;
                slot.style.backgroundImage = null;
            }

        }
    }
}
