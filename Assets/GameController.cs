using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public Tilemap obstacletile;//not walkable
    public Tilemap bgtile;//walkable
    public Tilemap buildingtile;//does not affect navmesh

    private static string saveDirectory = "Levels/";

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

    public GameObject h1, h2, h3, h4;

    private IEnumerator FadeOutAndDisable(Image image)
    {
        image.gameObject.SetActive(true);

        // 等待1秒钟
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;
        float fadeDuration = 1f;  // 渐变到透明的时间

        // 渐变透明度
        Color initialColor = image.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(initialColor.a, 0f, elapsedTime / fadeDuration);
            image.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // 完成后透明度为0
        image.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        // 禁用游戏对象
        image.gameObject.SetActive(false);
    }
    public void LoadLevel(int level)
    {
        h1.SetActive(false);
        h2.SetActive(false);
        h3.SetActive(false);
        h4.SetActive(false);

        switch (level)
        {
            case 1:
                StartCoroutine(FadeOutAndDisable(h1.GetComponent<Image>()));
                h1.SetActive(true);
                break;
            case 2:
                StartCoroutine(FadeOutAndDisable(h2.GetComponent<Image>()));
                break;
            case 4:
                StartCoroutine(FadeOutAndDisable(h3.GetComponent<Image>()));
                break;
            case 8:
                StartCoroutine(FadeOutAndDisable(h4.GetComponent<Image>()));
                break;

        }
        hasEnded = false;
        try
        {
            AimController.instance.shootSound.Stop();
            AimController.instance.reloadSound.Stop();
            AimController.instance.shootCooldown = 0;
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
        if (CurrentLevel == Consts.MaxLevel)
            UIController.instance.End();
        else
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
        Application.Quit();
    }
}
