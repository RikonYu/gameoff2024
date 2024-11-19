using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;
public class TilemapEditor : EditorWindow
{
    private static string saveDirectory = "Levels/";

    private static Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();


    [MenuItem("Tools/Save Tilemap Data")]
    public static void SaveTilemapData()
    {

        LevelData ans = new LevelData();
        ans.buildings = new List<BuildingData>();

        Tilemap colliderTiles = LevelEditorController.instance.ColliderTiles.GetComponent<Tilemap>();


        string levelString = LevelEditorController.instance.CurrentLevel.ToString();
        if (string.IsNullOrEmpty(levelString))
        {
            Debug.LogWarning("Invalid level number.");
            return;
        }

        List<Building> buildings = new List<Building>();
        buildings.AddRange(GameObject.FindObjectsOfType<Building>());
        foreach (var building in buildings)
            ans.buildings.Add(building.GetData());



        List<TileData> tileDataList = new List<TileData>();
        foreach (var position in colliderTiles.cellBounds.allPositionsWithin)
        {
            TileBase tile = colliderTiles.GetTile(position);
            if (tile != null)
            {
                tileDataList.Add(new TileData
                {
                    position = new Vector3Int(position.x, position.y, position.z),
                    tileName = tile.name,
                    spriteName = tile is Tile ? ((Tile)tile).sprite.name : null
                });
            }
        }

        ans.collisionTiles = tileDataList;

        ans.npcs = new List<NPCData>();
        var npcs = FindObjectsOfType<NPCController>();
        foreach(var npc in npcs)
        {
            ans.npcs.Add(new NPCData {
                prefabName=npc.transform.Find("sprite").name, 
                spriteName = npc.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite.name,
                position = npc.transform.position, 
                waypoints = npc.waypoints });
        }

        string json = JsonUtility.ToJson(ans);

        Debug.Log(json);
        string path = "Assets/Resources/Levels/" + levelString + ".json";
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();

        Debug.Log("Tilemap data saved to " + path);
    }

    [MenuItem("Tools/Load Tilemap Data")]
    public static void LoadTilemapData()
    {

        for (int i = 1; i <= Consts.MaxLevel; i++)
        {
            GameObject[] prefabs = Resources.LoadAll<GameObject>($"Prefabs/{i}");
            foreach (GameObject prefab in prefabs)
            {
                //Debug.Log(prefab.name);
                prefabDictionary[prefab.name] = prefab;
            }
        }

        List<GameObject> npcPrefabs = new List<GameObject>();
        npcPrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs"));

        int level = LevelEditorController.instance.CurrentLevel;
        string json = Resources.Load<TextAsset>(saveDirectory + level).text;

        LevelData tilemapData = JsonUtility.FromJson<LevelData>(json);

        ClearLevel();

        foreach (var building in tilemapData.buildings)
        {
            var obj = Instantiate(prefabDictionary[building.prefabName]);
            obj.transform.position = building.position;
        }

        var tilemap = LevelEditorController.instance.ColliderTiles.GetComponent<Tilemap>();
        tilemap.ClearAllTiles();
        foreach (var tileData in tilemapData.collisionTiles)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = Resources.Load<Sprite>("Sprites/" + tileData.spriteName);
            tilemap.SetTile(tileData.position, tile);
        }


        foreach (var i in npcPrefabs)
            Debug.Log(i.name);


        foreach (var npcData in tilemapData.npcs)
        {
            Debug.Log(npcData.spriteName);
            var npc = Instantiate(npcPrefabs.Find(x => x.name == npcData.spriteName));
            npc.transform.position = npcData.position;
            npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + npcData.spriteName);
            npc.GetComponent<NPCController>().waypoints = npcData.waypoints;
        }
    }

    [MenuItem("Tools/Clear Tilemap Data")]
    public static void ClearLevel()
    {
        foreach(var npc in GameObject.FindObjectsOfType<NPCController>())
            Object.DestroyImmediate(npc.gameObject);

        foreach (var building in GameObject.FindObjectsOfType<Building>())
            Object.DestroyImmediate(building.gameObject);

        LevelEditorController.instance.ColliderTiles.GetComponent<Tilemap>().ClearAllTiles();
    }
}