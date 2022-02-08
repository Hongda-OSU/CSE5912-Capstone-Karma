using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    //public string GetName();

    public float GetHP();

    //public float GetExperience();

    //public float GetAttackDamage();

    public void TakeDamage(float amount);




    // Methods below are for displaying enemy detection range, used in Editor.
    //public Vector3 GetTargetPosition();
    //public Transform GetTransform();
    //public float GetViewAngle();
    //public float GetViewRadius();
    //public float GetCloseDetectionDistance();
    //public bool FoundTarget();
    //public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal);
}
