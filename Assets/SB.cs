using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Debug.Log("???????????????????????????");
        var rb2d = GetComponent<Rigidbody2D>();
        rb2d.MovePosition(rb2d.position+ new Vector2(10f, 10f)*0.1f*Time.fixedDeltaTime);
    }
}
