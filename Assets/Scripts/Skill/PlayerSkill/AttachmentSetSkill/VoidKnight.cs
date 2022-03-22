using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class VoidKnight : PlayerSkill
    {
        [Header("Void Knight")]
        [SerializeField] private float distance;
        [SerializeField] private float speed;
        [SerializeField] private float baseDamage;
        [SerializeField] private float range = 1f;

        private float timeSince;
        private float totalTime;
        private bool isPerforming = false;

        private List<Enemy> damagedList = new List<Enemy>();

        private CharacterController cc;
        private GameObject player;

        private void Start()
        {
            player = PlayerManager.Instance.Player;
            cc = FPSControllerCC.Instance.CharacterController;

            MeleeAttack.Instance.meleeEvent.AddListener(Perform);
        }

        private void FixedUpdate()
        {
            if (!isPerforming)
                return;

            PerformDash();
        }

        private void Perform()
        {
            if (!isLearned || !isReady)
                return;

            StartDash();
        }
        private void StartDash()
        {
            StartCoolingdown();

            isPerforming = true;

            player.layer = LayerMask.NameToLayer("PlayerDashing");

            totalTime = distance / speed;
            timeSince = 0f;
        }

        private void PerformDash()
        {
            var delta = Time.deltaTime;

            cc.Move(player.transform.forward * speed * delta);

            var position = PlayerManager.Instance.Player.transform.position;
            var size = new Vector3(range, range, range);

            Collider[] hitColliders = Physics.OverlapBox(position, size);

            foreach (var hitCollider in hitColliders)
            {
                hitCollider.TryGetComponent(out Enemy enemy);
                if (enemy == null || !enemy.IsAlive)
                    continue;

                if (damagedList.Contains(enemy))
                    continue;

                var damage = new Damage(baseDamage, MeleeAttack.Instance.ElementType, PlayerStats.Instance, enemy);
                PlayerManager.Instance.PerformSkillDamage(enemy, damage);
                damagedList.Add(enemy);
            }

            timeSince += delta;

            if (timeSince > totalTime)
            {
                isPerforming = false;
                player.layer = LayerMask.NameToLayer("Player");
                damagedList.Clear();
            }
        }


        public override bool LevelUp()
        {
            var result = base.LevelUp();

            if (result)
                MeleeAttack.Instance.IsEnabled = false;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();

            MeleeAttack.Instance.IsEnabled = true;
        }

    }
}
