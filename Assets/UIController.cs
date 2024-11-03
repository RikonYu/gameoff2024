using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    public GameObject VisionCone;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void ShowCone()
    {
        VisionCone.SetActive(true);
    }
    public void HideCone()
    {
        VisionCone.SetActive(false);
    }

}
