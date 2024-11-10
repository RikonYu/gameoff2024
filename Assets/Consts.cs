using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consts
{

}

[System.Serializable]
public class LevelData
{
    [SerializeField]
    public List<TileData> buildingTiles;

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
