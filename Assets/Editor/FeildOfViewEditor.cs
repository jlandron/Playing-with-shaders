using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FeildOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fieldOfView = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fieldOfView.transform.position, Vector3.up, Vector3.forward, 360, fieldOfView.viewRadius);

        //Angle drawing
        Vector3 viewAngleA = fieldOfView.DirectionFromAngle(-fieldOfView.viewAngle / 2, false);
        Vector3 viewAngleB = fieldOfView.DirectionFromAngle(fieldOfView.viewAngle / 2, false);

        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleA * fieldOfView.viewRadius);
        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleB * fieldOfView.viewRadius);

        //draw lines to targets;
        Handles.color = Color.red;
        for (int i = 0; i < fieldOfView.visibleTargets.Count; i++)
        {
            Handles.DrawLine(fieldOfView.transform.position, fieldOfView.visibleTargets[i].position);
        }
    }
}
