using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class NPCWaypointSaver
{
    [MenuItem("Tools/Save All NPC Waypoints")]
    private static void SaveAllNPCWaypoints()
    {
        NPCController[] allNPCs = GameObject.FindObjectsOfType<NPCController>();
        int levelID = LevelManager.GetLevelID();

        string directoryPath = "Assets/Resources/Data/Waypoints";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        foreach (NPCController npc in allNPCs)
        {
            var data = npc.waypoints;

            string json = JsonUtility.ToJson(data, true);
            string filePath = $"{directoryPath}/{levelID}_{npc.characterID}.json";
            File.WriteAllText(filePath, json);
        }

        AssetDatabase.Refresh();
        Debug.Log($"All NPC waypoints saved to {directoryPath}.");
    }
}