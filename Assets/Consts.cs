using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Consts
{
    public static float PlayerMoveSpeed = 5f;
    public static float PlayerCrouchSpeedFactor = 0.5f;
    public static float ConeAngle = 5f;
    public static float ConeDistance = 10f;

    public static float CatchDistance = 1.0f;
}


[System.Serializable]
public class LevelData
{
    public List<ObstacleData> obstacles = new List<ObstacleData>();
    public List<NPCData> npcs = new List<NPCData>();
    public Vector2 walkableSize;

}



[System.Serializable]
public class ObstacleData
{
    public Vector3 position;
    public Vector3 scale;
    public string spriteName;
    public bool isWalkable;

    // Collider Data
    public string colliderType;
    public ColliderParameters colliderParameters;
}

[System.Serializable]
public class ColliderParameters
{
    // Common parameters
    public Vector2 offset;

    // BoxCollider2D parameters
    public Vector2 size;

    // CircleCollider2D parameters
    public float radius;

    // PolygonCollider2D parameters
    public List<Vector2> points;

    // EdgeCollider2D parameters
    public List<Vector2> edgePoints;
}

[System.Serializable]
public class NPCData
{
    public int characterID;
    public List<Vector2> waypoints;
}

[System.Serializable]
public class ColliderData
{
    public Vector2 size;
    public Vector2 offset;
}

public static class LevelDataIO
{
    private static string dataPath = "Assets/Resources/Levels";

    public static void SaveLevelData(int levelID, LevelData levelData)
    {
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        string json = JsonUtility.ToJson(levelData, true);
        string filePath = $"{dataPath}/Level_{levelID}.json";
        File.WriteAllText(filePath, json);
    }

    public static LevelData LoadLevelData(int levelID)
    {
        string filePath = $"{dataPath}/Level_{levelID}.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            return levelData;
        }
        return null;
    }
}