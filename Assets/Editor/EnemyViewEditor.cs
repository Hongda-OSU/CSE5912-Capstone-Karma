using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]

public class EnemyViewEditor : Editor
{
    public GameObject closestTarget;
    // float closestDistance = 9999;

    void OnSceneGUI()
    {
        EnemyController fow = (EnemyController)target;
        Handles.color = Color.white;
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawWireArc(fow.transform.position, Vector3.up, fow.DirFromAngle(-fow.viewAngle, false) + fow.transform.forward, fow.viewAngle, fow.viewRadius);
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.closeDetectionDistance);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        if (fow.foundTarget) {
            Handles.DrawLine(fow.transform.position, fow.GetTargetPosition());
        }
    }
}
