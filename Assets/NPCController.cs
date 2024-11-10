using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public int levelID;
    public int characterID;
    public List<Vector2> waypoints = new List<Vector2>();

    int currentWaypointInd = 0;

    bool isForward = true;
    NavMeshAgent agent;
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        agent.SetDestination(new Vector3(waypoints[currentWaypointInd].x, waypoints[currentWaypointInd].y, this.transform.position.z));
        Debug.Log($"{this.transform.parent.position}=>{waypoints[currentWaypointInd]}");
        if(Vector2.Distance(this.transform.parent.position, waypoints[currentWaypointInd])<= 0.01f){
            if (isForward)
                currentWaypointInd++;
            else
                currentWaypointInd--;
        }
        if (currentWaypointInd >= waypoints.Count)
        {
            currentWaypointInd = waypoints.Count - 1;
            isForward = false;
        }
        if (currentWaypointInd < 0)
        {
            currentWaypointInd = 0;
            isForward = true;
        }
        this.transform.parent.position = this.transform.position;
    }
}
