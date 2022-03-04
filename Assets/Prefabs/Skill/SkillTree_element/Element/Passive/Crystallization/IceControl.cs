using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

namespace CSE5912.PolyGamers
{
    public class IceControl : MonoBehaviour
    {
        [SerializeField] private ParticleSystem mainIce;
        [SerializeField] private ParticleSystem smallerIce;


        public IEnumerator WaitFor(float time)
        {
            yield return new WaitForSeconds(1f);

            var main = mainIce.main;
            main.simulationSpeed = 0f; 

            main = smallerIce.main;
            main.simulationSpeed = 0f;

            yield return new WaitForSeconds(time - 3f);

            main = mainIce.main;
            main.simulationSpeed = 1f;

            main = smallerIce.main;
            main.simulationSpeed = 1f;

            yield return new WaitForSeconds(2f);

        }
    }
}
