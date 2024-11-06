using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public int characterID;
    public List<Vector2> waypoints = new List<Vector2>();

    private int currentWaypointIndex = 0;

    private NavMeshAgent agent;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        MoveAlongWaypoints();
    }


    void MoveAlongWaypoints()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Vector2 targetPosition = waypoints[currentWaypointIndex];
        Vector2 currentPosition = transform.position;
        agent.SetDestination(targetPosition);

        if (Vector2.Distance(currentPosition, targetPosition) < 0.01f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; // 根据需要，可以改为停止移动
            }
        }
    }

    public NPCData GetData()
    {
        NPCData data = new NPCData();
        data.characterID = characterID;
        data.waypoints = new List<Vector2>(waypoints);
        return data;
    }

    public static void CreateFromData(NPCData data)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/NPCCharacter");
        if (prefab != null)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            NPCController npc = obj.GetComponent<NPCController>();
            npc.characterID = data.characterID;
            npc.waypoints = new List<Vector2>(data.waypoints);

            if (npc.waypoints.Count > 0)
            {
                obj.transform.position = npc.waypoints[0];
            }
        }
        else
        {
            Debug.LogError("NPC prefab not found in Resources/Prefabs.");
        }
    }
}