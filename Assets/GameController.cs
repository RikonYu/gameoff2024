﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;
using NavMeshPlus.Extensions;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public Tilemap obstacletile;//not walkable
    public Tilemap bgtile;//walkable
    public Tilemap buildingtile;//does not affect navmesh

    private static string saveDirectory = "Levels/";

    public NavMeshSurface navmesh;
    public GameObject Environment;

    public List<GameObject> npcPrefabs = new List<GameObject>();

    public List<Vector2> EventPositions= new List<Vector2>();

    private static Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    public int CurrentLevel;
    public void Start()
    {
        instance = this;
        npcPrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs"));
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs");
        foreach (GameObject prefab in prefabs)
        {
            //Debug.Log(prefab.name);
            prefabDictionary[prefab.name] = prefab;
        }
        this.LoadLevel(this.CurrentLevel);
        navmesh.BuildNavMesh();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel(int level)
    {
        string json = Resources.Load<TextAsset>(saveDirectory + level).text;

        LevelData tilemapData = JsonUtility.FromJson<LevelData>(json);


        foreach(var building in tilemapData.buildings)
        {
            var obj = Instantiate(prefabDictionary[building.prefabName]);
            obj.transform.position = building.position;
        }

        obstacletile.ClearAllTiles();
        foreach (var tileData in tilemapData.collisionTiles)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = Resources.Load<Sprite>("Sprites/" + tileData.spriteName);
            obstacletile.SetTile(tileData.position, tile);
        }

        foreach (var npcData in tilemapData.npcs)
        {
            var npc = Instantiate(npcPrefabs.Find(x => x.name == npcData.spriteName));
            npc.transform.position = npcData.position;
            //npc.transform.localPosition = new Vector2(0, -0.75f);
            //npc.transform.Find("collidersprite").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/charcollider");

            npc.GetComponent<NPCController>().waypoints = npcData.waypoints;
        }
        FillTilesInView();
    }

    void FillTilesInView()
    {
        Camera cam=Camera.main;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        Vector3Int bottomLeftCell = bgtile.WorldToCell(bottomLeft);
        Vector3Int topRightCell = bgtile.WorldToCell(topRight);

        if (bottomLeftCell.x > topRightCell.x)
        {
            int temp = bottomLeftCell.x;
            bottomLeftCell.x = topRightCell.x;
            topRightCell.x = temp;
        }
        if (bottomLeftCell.y > topRightCell.y)
        {
            int temp = bottomLeftCell.y;
            bottomLeftCell.y = topRightCell.y;
            topRightCell.y = temp;
        }

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = Resources.Load<Sprite>("Sprites/ground");

        // 遍历 Cell 坐标范围并设置 Tile
        for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
        {
            for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                bgtile.SetTile(cellPosition, tile);
            }
        }
    }

    public void CreateBloodAt(Vector2 pos)
    {
        this.EventPositions.Add(pos);
        Instantiate(prefabDictionary["blood"], pos, Quaternion.identity);
    }
}
