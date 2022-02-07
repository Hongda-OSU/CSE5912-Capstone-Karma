using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Skeleton_3))]

public class EnemyViewEditor : Editor
{
    public GameObject closestTarget;
    // float closestDistance = 9999;

    void OnSceneGUI()
    {
        Skeleton_3 fow = (Skeleton_3)target;
        Handles.color = Color.green;
        Vector3 viewAngleA = fow.DirFromAngle(-fow.GetViewAngle() / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.GetViewAngle() / 2, false);

        // Test code.
        // Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, 360, 10);

        Handles.DrawWireArc(fow.GetTransform().position, Vector3.up, fow.DirFromAngle(-fow.GetViewAngle(), false) + fow.GetTransform().forward, fow.GetViewAngle(), fow.GetViewRadius());
        Handles.DrawWireArc(fow.GetTransform().position, Vector3.up, Vector3.forward, 360, fow.GetCloseDetectionDistance());

        Handles.DrawLine(fow.GetTransform().position, fow.GetTransform().position + viewAngleA * fow.GetViewRadius());
        Handles.DrawLine(fow.GetTransform().position, fow.GetTransform().position + viewAngleB * fow.GetViewRadius());

        Handles.color = Color.red;
        if (fow.FoundTarget()) {
            Handles.DrawLine(fow.GetTransform().position, fow.GetTargetPosition());
        }
    }
}
