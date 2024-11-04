using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public int characterID;
    public List<Vector2> waypoints = new List<Vector2>(); 
    public float speed = 2f;

    private int currentWaypointIndex = 0;
    NavMeshAgent agent;
    bool isWalkingForward = true;

    void Awake()
    {
        AssignCharacterID();
    }

    void Start()
    {
        LoadWaypoints();
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(transform.position);
        Debug.Log($"waypoint count: {waypoints.Count}");

    }

    void Update()
    {
        MoveAlongWaypoints();
    }

    void AssignCharacterID()
    {
        NPCController[] existingNPCs = FindObjectsOfType<NPCController>();
        characterID = existingNPCs.Length;
    }

    void LoadWaypoints()
    {
        int levelID = LevelManager.GetLevelID();
        string path = $"Data/Waypoints/{levelID}_{characterID}";
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (textAsset != null)
        {
            List<Vector2> data = JsonUtility.FromJson<List<Vector2>>(textAsset.text);
            waypoints = data;
        }
        else
        {
            waypoints = new List<Vector2>();
        }
    }

    void MoveAlongWaypoints()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Vector2 targetPosition = waypoints[currentWaypointIndex];
        Vector2 currentPosition = transform.position;
        float step = speed * Time.deltaTime;
        //transform.position = Vector2.MoveTowards(currentPosition, targetPosition, step);

        agent.SetDestination(targetPosition);


        if (Vector2.Distance(currentPosition, targetPosition) < 0.01f)
        {
            if (isWalkingForward)
                currentWaypointIndex++;
            else
                currentWaypointIndex--;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = waypoints.Count - 1;
                isWalkingForward = false;
            }
            if (currentWaypointIndex<0)
            {
                currentWaypointIndex = 0;
                isWalkingForward = true;
            }
                
        }
    }
}