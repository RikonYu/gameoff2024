using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelEditorController : MonoBehaviour
{
    public static LevelEditorController instance;
    public GameObject ColliderTiles;

    public int CurrentLevel;
    public void OnEnable()
    {
        instance = this;
    }
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
