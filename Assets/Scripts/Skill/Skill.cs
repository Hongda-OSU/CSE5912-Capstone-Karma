using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class Skill : MonoBehaviour
    {
        [SerializeField] protected float cooldown;
        [SerializeField] protected float timeSince;
        [SerializeField] protected bool isReady = false;

        public void StartCoolingdown()
        {
            StartCoroutine(CoolDown());
        }

        private IEnumerator CoolDown()
        {
            isReady = false;

            timeSince = 0f;
            while (timeSince < cooldown)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            isReady = true;
        }

        public float Cooldown { get { return cooldown; } }
        public float TimeSince { get { return timeSince; } }
        public bool IsReady { get { return isReady; } }

    }
}

