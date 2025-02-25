using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResizablePlane))]
public class ResizablePlaneEditor : Editor
{
    private ResizablePlane resizablePlane;

    private void OnSceneGUI()
    {
        resizablePlane = (ResizablePlane)target;

        // Draw handles for each corner
        for (int i = 0; i < resizablePlane.corners.Length; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 worldCorner = resizablePlane.transform.TransformPoint(resizablePlane.corners[i]);
            Vector3 newPosition = Handles.PositionHandle(worldCorner, Quaternion.identity);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(resizablePlane, "Move Plane Corner");
                resizablePlane.corners[i] = resizablePlane.transform.InverseTransformPoint(newPosition);
                resizablePlane.UpdatePlane();
            }
        }

        // Draw lines between corners
        Handles.color = Color.cyan; // Set line color
        Vector3[] worldCorners = new Vector3[resizablePlane.corners.Length];
        for (int i = 0; i < resizablePlane.corners.Length; i++)
        {
            worldCorners[i] = resizablePlane.transform.TransformPoint(resizablePlane.corners[i]);
        }
        Handles.DrawPolyLine(worldCorners);

        // Close the polygon by connecting the last corner to the first
        Handles.DrawLine(worldCorners[worldCorners.Length - 1], worldCorners[0]);


        //DEBUGGING ---------------------------------
        // Draw center point
        Vector3 worldCenter = resizablePlane.transform.TransformPoint(resizablePlane.centerPoint);
        Handles.color = Color.red;
        Handles.SphereHandleCap(0, worldCenter, Quaternion.identity, 0.5f, EventType.Repaint);
    }
}