using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class LevelEditorWindow : EditorWindow
{
    private int currentLevelID = 1;

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Editor", EditorStyles.boldLabel);

        // Level ID
        currentLevelID = EditorGUILayout.IntField("Current Level ID:", currentLevelID);

        GUILayout.Space(10);

        // Level Control Buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Level"))
        {
            NewLevel();
        }
        if (GUILayout.Button("Load Level"))
        {
            LoadLevel();
        }
        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
    }

    private void NewLevel()
    {
        if (EditorUtility.DisplayDialog("New Level", "Are you sure you want to create a new level? Unsaved changes will be lost.", "Yes", "No"))
        {
            ClearScene();
        }
    }

    private void LoadLevel()
    {
        if (EditorUtility.DisplayDialog("Load Level", "Are you sure you want to load the level? Unsaved changes will be lost.", "Yes", "No"))
        {
            ClearScene();
            LoadLevelData(currentLevelID);
        }
    }

    private void SaveLevel()
    {
        SaveLevelData(currentLevelID);
        EditorUtility.DisplayDialog("Save Level", "Level saved successfully.", "OK");
    }

    private void ClearScene()
    {
        // Remove all NPCs and Obstacles from the scene
        foreach (var npc in FindObjectsOfType<NPCController>())
        {
            DestroyImmediate(npc.gameObject);
        }
    }

    private void LoadLevelData(int levelID)
    {
        LevelData levelData = LevelDataIO.LoadLevelData(levelID);
        if (levelData != null)
        {

            // Load NPCs
            foreach (var npcData in levelData.npcs)
            {
                NPCController.CreateFromData(npcData);
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Load Level", $"Level {levelID} data not found.", "OK");
        }
    }

    private void SaveLevelData(int levelID)
    {
        LevelData levelData = new LevelData();

        // Save NPCs
        var npcs = FindObjectsOfType<NPCController>();
        var obstacles = FindObjectsOfType<Obstacle>();
        foreach (var npc in npcs)
        {
            levelData.npcs.Add(npc.GetData());
        }

        foreach(var obstacle in obstacles)
        {
            levelData.obstacles.Add(obstacle.GetData());
        }

        var walkable = GameObject.Find("Walkable");
        levelData.walkableSize = new Vector2(walkable.transform.localScale.x, walkable.transform.localScale.y);

        LevelDataIO.SaveLevelData(levelID, levelData);
    }
}