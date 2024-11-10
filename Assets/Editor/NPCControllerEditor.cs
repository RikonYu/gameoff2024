using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(NPCController))]
public class NPCWaypointEditor : Editor
{
    private NPCController npcController;
    private bool showWaypointsList = false;
    private int selectedWaypointIndex = -1;

    private void OnEnable()
    {
        npcController = (NPCController)target;

        if (EditorSceneManager.GetActiveScene().name == "LevelEditor")
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
    }

    private void OnDisable()
    {
        if (EditorSceneManager.GetActiveScene().name == "LevelEditor")
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // 仅在Level Editor场景中显示Waypoints编辑功能
        if (EditorSceneManager.GetActiveScene().name == "LevelEditor")
        {
            GUILayout.Space(10);
            GUILayout.Label("Waypoints Editor", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Waypoint"))
            {
                if(npcController.waypoints.Count==0)
                AddWaypointAtPosition(npcController.transform.position);
                else
                    AddWaypointAtPosition(npcController.waypoints[npcController.waypoints.Count-1]);
            }

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
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (npcController.waypoints == null || !npcController.isActiveAndEnabled)
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
            float handleSize = HandleUtility.GetHandleSize(waypointPosition) * 0.1f;

            Vector3[] rectangleCorners = new Vector3[4];
            float halfSize = 0.25f; 

            rectangleCorners[0] = waypointPosition + new Vector3(-halfSize, -halfSize, 0);
            rectangleCorners[1] = waypointPosition + new Vector3(-halfSize, halfSize, 0);
            rectangleCorners[2] = waypointPosition + new Vector3(halfSize, halfSize, 0);
            rectangleCorners[3] = waypointPosition + new Vector3(halfSize, -halfSize, 0);

            Handles.DrawSolidRectangleWithOutline(rectangleCorners, new Color(1, 0, 0, 0.3f), Color.red);

            // 绘制按钮来选择waypoint
            if (Handles.Button(waypointPosition, Quaternion.identity, handleSize, handleSize, Handles.DotHandleCap))
            {
                selectedWaypointIndex = i;
                Repaint();
            }

            // 编辑选中的waypoint
            if (selectedWaypointIndex == i)
            {
                Vector3 newPos = Handles.PositionHandle(waypointPosition, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(npcController, "Move Waypoint");
                    npcController.waypoints[i] = new Vector2(Mathf.Round(newPos.x)+0.5f, Mathf.Round(newPos.y)+0.5f);
                }
            }

            Handles.Label(waypointPosition + Vector3.up * 0.2f, $"Waypoint {i}");
        }

        // 绘制Waypoints之间的连线
        Handles.color = Color.red;

        // 连接NPCController位置到第一个waypoint
        if (npcController.waypoints.Count > 0)
        {
            Vector3 startPosition = npcController.transform.position;
            Vector3 firstWaypoint = new Vector3(npcController.waypoints[0].x, npcController.waypoints[0].y, npcController.transform.position.z);
            Handles.DrawAAPolyLine(5f, startPosition, firstWaypoint);
        }

        // 连接相邻的waypoints
        for (int i = 0; i < npcController.waypoints.Count - 1; i++)
        {
            Vector3 pointA = new Vector3(npcController.waypoints[i].x, npcController.waypoints[i].y, npcController.transform.position.z);
            Vector3 pointB = new Vector3(npcController.waypoints[i + 1].x, npcController.waypoints[i + 1].y, npcController.transform.position.z);
            Handles.DrawAAPolyLine(5f, pointA, pointB);
        }

        // 删除选中的Waypoint（按Delete键）
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
}
