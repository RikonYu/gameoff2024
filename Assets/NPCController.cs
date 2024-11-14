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
        if(this.waypoints.Count ==0)
            agent.updatePosition = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (this.waypoints.Count == 0)
            return;
        agent.SetDestination(new Vector3(waypoints[currentWaypointInd].x, waypoints[currentWaypointInd].y, this.transform.position.z));
        //Debug.Log($"{this.transform.position}=>{waypoints[currentWaypointInd]}");
        if(Vector2.Distance(this.transform.position, waypoints[currentWaypointInd])<= 0.05f){
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
    }
    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
