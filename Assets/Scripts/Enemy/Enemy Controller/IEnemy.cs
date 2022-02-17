using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public interface IEnemy
    {
        //public string GetName();

        public float GetHealth();
        public float GetMaxHealth();

        //public float GetExperience();

        //public float GetAttackDamage();

        public void TakeDamage(float amount);

        public bool FoundTarget();



        // Methods below are for displaying enemy detection range, used in Editor.
        //public Vector3 GetTargetPosition();
        //public Transform GetTransform();
        //public float GetViewAngle();
        //public float GetViewRadius();
        //public float GetCloseDetectionDistance();
        //public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal);
    }
}