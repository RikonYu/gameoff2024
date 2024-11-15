using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consts
{
    public static int DetectionRange = 1000;
    public static int DetectionAngle = 30;
    public static float GuardSpeed = 3f;
    public static float GuardWarnedSpeed = 6f;
    public static float WalkDistance = 0.002f;
}
[System.Serializable]
public class BuildingData
{
    public Vector2 position;
    public string prefabName;
}

[System.Serializable]
public class LevelData
{
    [SerializeField]
    public List<BuildingData> buildings;

    [SerializeField]
    public List<TileData> collisionTiles;

    [SerializeField]
    public List<NPCData> npcs;
}

[System.Serializable]
public class NPCData
{
    public string prefabName;
    public string spriteName;
    public Vector2 position;
    public List<Vector2> waypoints;
}
[System.Serializable]
public class TileData
{
    public Vector3Int position;
    public string tileName;
    public string spriteName;
}
