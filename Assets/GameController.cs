using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public NavMeshSurface Surface2D;
    public GameObject Walkable;
    public int CurrentLevelID = 1;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        this.LoadLevel(CurrentLevelID);
        Surface2D.BuildNavMesh();
    }

    void LoadLevel(int levelID)
    {
        LevelData levelData = LevelDataIO.LoadLevelData(levelID);
        if (levelData != null)
        {

            // Load NPCs
            foreach (var npcData in levelData.npcs)
            {
                NPCController.CreateFromData(npcData);
            }
            foreach (var obstacleData in levelData.obstacles)
                Obstacle.CreateFromData(obstacleData);
        }
        else
        {
            Debug.LogError($"Level data for level {levelID} not found.");
        }

        Walkable.transform.localScale = new Vector3(levelData.walkableSize.x, levelData.walkableSize.y, 1);
        Walkable.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
