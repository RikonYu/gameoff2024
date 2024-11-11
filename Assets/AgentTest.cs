using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentTest : MonoBehaviour
{
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(new Vector2(10f, 0f));
    }
}
