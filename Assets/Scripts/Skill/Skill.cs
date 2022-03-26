using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class Skill : MonoBehaviour
    {
        [SerializeField] protected float cooldown;
        [SerializeField] protected float timeSincePerformed;
        [SerializeField] protected bool isReady = false;

        public virtual void StartCoolingdown()
        {
            StartCoroutine(CoolDown());
        }

        private IEnumerator CoolDown()
        {
            isReady = false;

            timeSincePerformed = 0f;
            while (timeSincePerformed < cooldown)
            {
                timeSincePerformed += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            isReady = true;
        }

        public float Cooldown { get { return cooldown; } set { cooldown = value; } }
        public float TimeSincePerformed { get { return timeSincePerformed; } set { timeSincePerformed = value; } }
        public bool IsReady { get { return isReady; } set { isReady = value; } }

    }
}

