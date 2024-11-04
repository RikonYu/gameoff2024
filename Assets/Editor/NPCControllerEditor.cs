using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(NPCController))]
public class NPCControllerEditor : Editor
{
    private NPCController npcController;
    private bool showWaypointsList = false;
    private int selectedWaypointIndex = -1;

    private void OnEnable()
    {
        npcController = (NPCController)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Waypoints Editor", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Waypoint"))
        {
            AddWaypointAtPosition(npcController.transform.position);
        }

        if (GUILayout.Button("Clear Waypoint"))
            npcController.waypoints.Clear();

        GUILayout.Space(10);

        showWaypointsList = EditorGUILayout.Foldout(showWaypointsList, "Show Waypoints List");
        if (showWaypointsList)
        {
            EditorGUI.indentLevel++;
            if (npcController.waypoints != null)
            {
                for (int i = 0; i < npcController.waypoints.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Waypoint {i}", GUILayout.Width(100));
                    EditorGUILayout.Vector2Field("", npcController.waypoints[i]);
                    if (GUILayout.Button("Delete"))
                    {
                        Undo.RecordObject(npcController, "Delete Waypoint");
                        npcController.waypoints.RemoveAt(i);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Assign Character IDs"))
        {
            AssignCharacterIDs();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Application.isPlaying)
            return;

        if (npcController.waypoints == null)
            return;

        Event e = Event.current;

        if (e.control && e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 worldPosition = ray.origin;
            Undo.RecordObject(npcController, "Add Waypoint");
            AddWaypointAtPosition(new Vector2(worldPosition.x, worldPosition.y));
            e.Use();
        }

        for (int i = 0; i < npcController.waypoints.Count; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 waypointPosition = new Vector3(npcController.waypoints[i].x, npcController.waypoints[i].y, npcController.transform.position.z);
            float handleSize = HandleUtility.GetHandleSize(waypointPosition) * 0.5f;

            if (Handles.Button(waypointPosition, Quaternion.identity, handleSize, handleSize, Handles.DotHandleCap))
            {
                selectedWaypointIndex = i;
                Repaint();
            }

            if (selectedWaypointIndex == i)
            {
                Vector3 newPos = Handles.PositionHandle(waypointPosition, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(npcController, "Move Waypoint");
                    npcController.waypoints[i] = new Vector2(newPos.x, newPos.y);
                }
            }

            Handles.Label(waypointPosition + Vector3.up * 0.2f, $"Waypoint {i}");
        }

        Handles.color = Color.red;
        if(npcController.waypoints.Count>0)
            Handles.DrawLine(npcController.transform.position, npcController.waypoints[0], 5f);

        for (int i = 0; i < npcController.waypoints.Count - 1; i++)
        {
            Vector3 pointA = new Vector3(npcController.waypoints[i].x, npcController.waypoints[i].y, npcController.transform.position.z);
            Vector3 pointB = new Vector3(npcController.waypoints[i + 1].x, npcController.waypoints[i + 1].y, npcController.transform.position.z);
            Handles.DrawLine(pointA, pointB, 5f);
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
        {
            if (selectedWaypointIndex >= 0 && selectedWaypointIndex < npcController.waypoints.Count)
            {
                Undo.RecordObject(npcController, "Delete Waypoint");
                npcController.waypoints.RemoveAt(selectedWaypointIndex);
                selectedWaypointIndex = -1;
                e.Use();
            }
        }
    }

    private void AddWaypointAtPosition(Vector2 position)
    {
        npcController.waypoints.Add(position);
        EditorUtility.SetDirty(npcController);
    }

    private void AssignCharacterIDs()
    {
        NPCController[] allNPCs = FindObjectsOfType<NPCController>();
        for (int i = 0; i < allNPCs.Length; i++)
        {
            allNPCs[i].characterID = i + 1;
            EditorUtility.SetDirty(allNPCs[i]);
        }
    }
}