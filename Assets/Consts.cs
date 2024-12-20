using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consts
{
    public static int DetectionRange = 10;
    public static int DetectionPixelSize = 100;
    public static int DetectionAngle = 30;
    public static float GuardSpeed = 1.5f;
    public static float MobSpeed = 0.75f;
    public static float MobStandTime = 2f;
    public static float GuardStandTime = 1f;

    public static float CarSpeed = 4f;


    public static float GuardWarnedMultiplier = 1.5f;
    public static float GuardWarnDistance = 0.2f;
    public static float WalkDistance = 0.002f;
    public static float AimCircleSize = 175f;

    public static float ShootCooldown = 3f;

    public static float LoseMenuTime = 1f;

    

    public static int MaxLevel = 9;
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
