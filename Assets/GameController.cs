using System.Collections;
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


    public bool HasBoss = false;

    public int LastStageBoss = 4;

    // Start is called before the first frame update
    public int CurrentLevel;
    int LivingEnemyCount;
    int livingBossCount;

    System.Action SpecialRule;

    public void Start()
    {
        instance = this;

        npcPrefabs.AddRange(Resources.LoadAll<GameObject>($"Prefabs"));
        for (int i=1;i<=Consts.MaxLevel;i++)
        {
            GameObject[] prefabs = Resources.LoadAll<GameObject>($"Prefabs/{i}");
            foreach (GameObject prefab in prefabs)
            {
                //Debug.Log(prefab.name);
                prefabDictionary[prefab.name] = prefab;
            }
        }
        foreach (GameObject prefab in Resources.LoadAll<GameObject>($"Prefabs"))
        {
            //Debug.Log(prefab.name);
            prefabDictionary[prefab.name] = prefab;
        }

        this.LoadLevel(this.CurrentLevel);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        SpecialRule?.Invoke();
    }

    public void NextLevel() { LoadLevel(++this.CurrentLevel); }
    public void ReplayLevel() { LoadLevel(this.CurrentLevel); }
    public void LoadLevel(int level)
    {
        hasEnded = false;
        try
        {
            AimController.instance.shootSound.Stop();
            AimController.instance.reloadSound.Stop();
        }
        catch
        {
            
        }
        Time.timeScale = 1;
        HasBoss = false;
        if (level > Consts.MaxLevel)
        {
            Debug.Log("WIN!");
            return;
        }
        DoNotWarn = false;
        string json = Resources.Load<TextAsset>(saveDirectory + level).text;

        LevelData tilemapData = JsonUtility.FromJson<LevelData>(json);

        LivingEnemyCount = 0;
        this.EventPositions.Clear();
        foreach (var npc in FindObjectsOfType<NPCController>())
            DestroyImmediate(npc.gameObject);
        foreach (var b in FindObjectsOfType<SpriteRenderer>())
            if (b.sprite.name == "blood")
                DestroyImmediate(b.gameObject);
        foreach (var c in FindObjectsOfType<Building>())
            DestroyImmediate(c.gameObject);

        foreach (var building in tilemapData.buildings)
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

        
        FillTilesInView();
        navmesh.BuildNavMesh();

        foreach (var npcData in tilemapData.npcs)
        {
            var npc = Instantiate(npcPrefabs.Find(x => x.name == npcData.spriteName));
            npc.transform.position = npcData.position;

            if (npc.GetComponent<NPCController>().IsBoss)
                livingBossCount++;
            LivingEnemyCount++;
            npc.GetComponent<NPCController>().waypoints = npcData.waypoints;
        }


        SpecialRule = SpecialRules.ApplyLevelUpdate(this.CurrentLevel);
        SpecialRules.ApplyLevelInit(this.CurrentLevel).Invoke();

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
    bool hasEnded=false;
    public void Lose()
    {
        if (hasEnded) return;
        hasEnded = true;
        UIController.instance.ShowLose();
        Debug.Log("LOSE!");
    }

    public void Win()
    {
        if (hasEnded) return;
        hasEnded = true;
        Time.timeScale = 0;
        UIController.instance.ShowWin();
    }
    public void CharacterDead(GameObject obj, bool IsBoss)
    {
        LivingEnemyCount--;
        if (IsBoss)
            livingBossCount--;
        this.CreateBloodAt(obj.transform.position);
        if (LivingEnemyCount <= 0 || (this.HasBoss&& livingBossCount<=0))
        {
            Win();
        }
    }

    void CreateBloodAt(Vector2 pos)
    {

        this.EventPositions.Add(pos);
        var obj = Instantiate(prefabDictionary["blood"], pos, Quaternion.identity);
        obj.name = "blood";
    }

    public bool DoNotWarn = false;
    public void Warn()
    {
        if (DoNotWarn)
            return;
        Lose();
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
    }
    public void resumeGame()
    {
        Time.timeScale = 1;
    }
    public void returnToMain()
    {

    }
}
