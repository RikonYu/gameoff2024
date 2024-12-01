using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool IsMoving = true;
    public Vector2 StopPosition;
    public Vector2 EndPosition;
    public float StopTime;
    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate()
    {
        if (!IsMoving)
            return;
        if (Vector2.Distance(this.transform.position, EndPosition)<=0.1f)
            this.gameObject.SetActive(false);
        if (StopTime >= 0f)
        {
            if(Vector2.Distance(this.transform.position, StopPosition) <= 0.01f)
            {
                this.StopTime -= Time.deltaTime;
            }
            else
                transform.position = Vector2.MoveTowards(transform.position, StopPosition, Consts.CarSpeed * Time.deltaTime);
        }
        else
            transform.position = Vector2.MoveTowards(transform.position, EndPosition, Consts.CarSpeed * Time.deltaTime);
    }
}
