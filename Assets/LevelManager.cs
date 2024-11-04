using UnityEngine;

[ExecuteInEditMode]
public class LevelManager : MonoBehaviour
{
    public int levelID = 1; // 默认关卡ID为1

    public static LevelManager instance;

    void Awake()
    {
        instance = this;
        
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        // Ensure instance is set in edit mode
        if (!Application.isPlaying)
        {
            instance = this;
        }
    }
    void OnValidate()
    {
        // Ensure instance is updated when properties change in the inspector
        if (!Application.isPlaying)
        {
            instance = this;
        }
    }

    public static int GetLevelID()
    {
        if (instance != null)
        {
            return instance.levelID;
        }
        else
        {
            // Try to find LevelManager in the scene
            instance = FindObjectOfType<LevelManager>();
            if (instance != null)
            {
                return instance.levelID;
            }
            else
            {
                Debug.LogError("LevelManager instance not found!");
                return -1;
            }
        }
    }
}