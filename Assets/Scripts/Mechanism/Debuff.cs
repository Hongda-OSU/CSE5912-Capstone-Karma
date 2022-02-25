using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class Debuff : MonoBehaviour
    {
        [SerializeField] protected DebuffType type;

        [SerializeField] protected int stack = 0;
        [SerializeField] protected int maxStack = 5;

        [SerializeField] protected float duration = 5f;
        [SerializeField] protected float timeSince = 0f;

        [SerializeField] protected Sprite icon;

        [SerializeField] protected Enemy target;

        public enum DebuffType
        {
            Burned,
            Frozen,
            Electrocuted,
            Infected,
        }

        protected void Awake()
        {
            target = transform.parent.GetComponent<Enemy>();
        }

        public void StackUp()
        {
            // increment stack
            stack = Mathf.Clamp(stack + 1, 0, maxStack);

            // refresh duration
            timeSince = 0f;

            if (stack == 1)
            {
                StartCoroutine(Perform());
            }
        }

        protected abstract IEnumerator Perform();

        public DebuffType Type { get { return type; } }
        public int Stack { get { return stack; } }
        public Sprite Icon { get { return icon; } }
    }
}
