using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class NPCEditor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "LevelEditor")
            return;
        this.transform.position = new Vector2(Mathf.Round(this.transform.position.x-0.5f)+0.5f, Mathf.Round(this.transform.position.y-0.5f)+0.5f);
    }
}
