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
    public bool isMoving = true;
    public bool isWarned = false;
    public Vector2 warnedPosition;
    SpriteRenderer sprite;
    Animator animator;
    float speed;
    string unitName;
    public bool IsBoss { get => this.transform.Find("sprite").gameObject.GetComponent<SpriteRenderer>().sprite.name.Contains("boss");}

    void Start()
    {
        this.Init();
    }
    public void Init()
    {
        this.lastPosition = this.transform.position;


        

        animator = this.transform.Find("sprite").gameObject.GetComponent<Animator>();
        sprite = this.transform.Find("sprite").gameObject.GetComponent<SpriteRenderer>();
        string name = sprite.sprite.name;
        if (name.Contains("mob"))
        {
            maxWaitingTime = Consts.MobStandTime;
            speed = Consts.MobSpeed;
        }
        else
        {
            speed = Consts.GuardSpeed;
            maxWaitingTime = Consts.GuardStandTime;
        }

        waitingTime = maxWaitingTime;

        unitName = sprite.sprite.name;
        if (name.Contains("guard"))
        {
            this.DrawDetection(Consts.DetectionRange * Consts.DetectionPixelSize, Consts.DetectionAngle);
        }
        else
            this.transform.Find("detection").gameObject.SetActive(false);
    }
    public float maxWaitingTime = 0f;
    public float waitingTime = 0f;
    private void Update()
    {
        Vector2 currentPosition = this.transform.position;
        if ((currentPosition - lastPosition).magnitude >= Consts.WalkDistance*speed)
        {
            Vector2 diff = currentPosition - lastPosition;
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            this.transform.Find("detection").localRotation = Quaternion.Euler(0, 0, angle);
        }

        Vector2 directionToTarget = currentPosition - lastPosition;
        if (directionToTarget.magnitude >= 1e-4)
        {
            
                float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
                if (angle < 0)
                    angle += 360;

                if (150 <= angle && angle <= 210)
                    this.transform.Find("sprite").GetComponent<SpriteRenderer>().flipX = true;
                else
                    this.transform.Find("sprite").GetComponent<SpriteRenderer>().flipX = false;


                if (angle > 45 && angle < 135)
                {
                    animator.Play($"{unitName}walkingN");
                }
                else if (angle > 225 && angle < 315)
                {
                    animator.Play($"{unitName}walkingS");
                }
                else
                {
                    animator.Play($"{unitName}walkingside");
                }
            

        }

        
        if(!GameController.instance.DoNotWarn)
        foreach(var i in GameController.instance.EventPositions)
        {
            if (IsTargetInSector(i))
            {
                this.isWarned = true;
                warnedPosition = i;
            }

        }

        if (this.isWarned)
        {
            
            animator.speed = 0.5f* Consts.GuardWarnedMultiplier * this.speed;
        }
        else
        {
            animator.speed = 0.5f * this.speed;
        }
        lastPosition = this.transform.position;
    }

    void SetDestination(Vector2 pos)
    {
        var rb2D = GetComponent<Rigidbody2D>();
        var np = Vector2.MoveTowards(transform.position, pos, this.speed * Time.fixedDeltaTime);
        rb2D.MovePosition(np);
        //Debug.Log($"from {this.transform.position} to {np}");
        //Physics.SyncTransforms();


    }
    private void FixedUpdate()
    {
        if (!this.isMoving)
        {
            this.animator.speed = 0;
            return;
        }
        else
            this.animator.speed = 1f;
            
        if(this.isWarned)
        {
            StartAnim();
            SetDestination(warnedPosition);
            //transform.position = Vector2.MoveTowards(transform.position, warnedPosition, this.speed * Time.fixedDeltaTime);
            this.speed = Consts.GuardSpeed * Consts.GuardWarnedMultiplier;
            return;
        }
        
        if (this.waypoints.Count == 0)
            return;

        SetDestination(waypoints[currentWaypointInd]);
        //Debug.Log($"{transform.position}, {waypoints[currentWaypointInd]}, {Time.fixedDeltaTime}, {this.speed * Time.fixedDeltaTime}, {Vector2.MoveTowards(transform.position, waypoints[currentWaypointInd], this.speed * Time.fixedDeltaTime)}");
        //transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointInd], this.speed * Time.fixedDeltaTime);

        if (Vector2.Distance(this.transform.position, waypoints[currentWaypointInd])<= 0.1f){
/*            agent.radius = 1e-4f;
            agent.height = 1e-4f;*/
            if (this.waitingTime >= this.maxWaitingTime)
            {
/*                if (this.IsBoss)
                    Debug.Log($"{this.waitingTime}, {currentWaypointInd}");*/
                this.waitingTime = 0f;
                StartAnim();

                if (isForward)
                    currentWaypointInd++;
                else
                    currentWaypointInd--;

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
            else
            {
                waitingTime += Time.fixedDeltaTime;
                StopAnim();
            }
                
            
        }
        


    }
    void StopAnim()
    {
        animator.speed = 0;
        //animator.Play(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name,0,0);
    }
    void StartAnim()
    {
        if (this.isWarned)
        {

            animator.speed = 0.5f * Consts.GuardWarnedMultiplier * this.speed;
        }
        else
        {
            animator.speed = 0.5f * this.speed;
        }
    }
    public void Kill()
    {
        Destroy(this.gameObject);
        GameController.instance.CharacterDead(this.gameObject, this.IsBoss);
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

        Vector2 movementDirection = (Vector2)this.transform.position - lastPosition;
        if (movementDirection == Vector2.zero)
        {
            movementDirection = Vector2.right;
        }

        float sectorDirection = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;

        Vector2 directionToTarget = targetPosition - sectorCenter;
        float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        float angleDifference = Mathf.DeltaAngle(sectorDirection, angleToTarget);

        return Mathf.Abs(angleDifference) <= halfAngle;
    }
    


}
