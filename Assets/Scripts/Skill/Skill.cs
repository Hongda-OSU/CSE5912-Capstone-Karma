using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class Skill : MonoBehaviour
    {
        [SerializeField] protected float cooldown;
        [SerializeField] protected bool isReady = false;

        protected void StartCoolingdown()
        {
            StartCoroutine(CoolDown());
        }

        private IEnumerator CoolDown()
        {
            isReady = false;

            yield return new WaitForSeconds(cooldown);

            isReady = true;
        }

        public float Cooldown { get { return cooldown; } }
        public bool IsReady { get { return isReady; } }

    }
}

