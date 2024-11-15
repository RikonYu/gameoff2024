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

    Vector2 lastPosition;

    bool isForward = true;
    bool isWarned = false;
    Vector2 warnedPosition;
    NavMeshAgent agent;
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        this.lastPosition = this.transform.position;
        if(this.waypoints.Count ==0)
            agent.updatePosition = false;
        agent.updateUpAxis = false;
        agent.speed = Consts.GuardSpeed;
        

        this.DrawDetection(Consts.DetectionRange, Consts.DetectionAngle);

    }

    private void Update()
    {
        Vector2 currentPosition = this.transform.position;
        Debug.Log((currentPosition - lastPosition).magnitude);
        if ((currentPosition - lastPosition).magnitude >= Consts.WalkDistance)
        {
            Vector2 diff = currentPosition - lastPosition;
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            this.transform.Find("detection").rotation = Quaternion.Euler(0, angle, 0);
        }
        lastPosition = this.transform.position;

        foreach(var i in GameController.instance.EventPositions)
        {
            if (IsTargetInSector(i))
            {
                this.isWarned = true;
                warnedPosition = i;
            }

        }
    }

    private void FixedUpdate()
    {
        if(this.isWarned)
        {
            agent.SetDestination(warnedPosition);
            this.agent.speed = Consts.GuardWarnedSpeed;
            return;
        }
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
        GameController.instance.CreateBloodAt(this.transform.position);
    }

    void DrawDetection(int length, int angle)
    {
        int radius = Mathf.RoundToInt(length);
        int diameter = radius * 2;
        Texture2D tex = new Texture2D(diameter, diameter, TextureFormat.ARGB32, false);
        Color[] fillColorArray = tex.GetPixels();
        for (int i = 0; i < fillColorArray.Length; i++)
        {
            fillColorArray[i] = Color.clear;
        }
        float halfAngleRad = angle * 0.5f * Mathf.Deg2Rad;
        Vector2 center = new Vector2(radius, radius);
        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                float dx = x - radius;
                float dy = y - radius;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                if (dist <= radius)
                {
                    float pixelAngle = Mathf.Atan2(dy, dx);
                    if (pixelAngle < 0)
                    {
                        pixelAngle += 2 * Mathf.PI;
                    }
                    float startAngle = -halfAngleRad;
                    float endAngle = halfAngleRad;
                    float adjustedAngle = pixelAngle > Mathf.PI ? pixelAngle - 2 * Mathf.PI : pixelAngle;
                    if (adjustedAngle >= startAngle && adjustedAngle <= endAngle)
                    {
                        int pixelIndex = y * diameter + x;
                        fillColorArray[pixelIndex] = Color.yellow;
                        fillColorArray[pixelIndex].a = 0.5f;
                    }
                }
            }
        }
        tex.SetPixels(fillColorArray);
        tex.Apply();
        Sprite sectorSprite = Sprite.Create(tex, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f));
        this.transform.Find("detection").GetComponent<SpriteRenderer>().sprite = sectorSprite;
    }

    bool IsTargetInSector(Vector2 targetPosition)
    {

        Vector2 sectorCenter = transform.position;
        float distance = Vector2.Distance(targetPosition, sectorCenter);

        if (distance > Consts.DetectionRange) return false;

        float halfAngle = Consts.DetectionAngle * 0.5f;
        float sectorDirection = transform.Find("detection").eulerAngles.z;
        Vector2 directionToTarget = targetPosition - sectorCenter;
        float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        float angleDifference = Mathf.DeltaAngle(sectorDirection, angleToTarget);

        return Mathf.Abs(angleDifference) <= halfAngle;
    }
}
