using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject WinMenu;
    public GameObject LoseMenu;
    public static UIController instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowPause()
    {
        PauseMenu.SetActive(true);
    }
    public void HidePause()
    {
        PauseMenu.SetActive(false);
    }
    public void ShowWin()
    {
        WinMenu.SetActive(true);
    }
    public void HideWin()
    {
        WinMenu.SetActive(false);
    }
    public void ShowLose()
    {
        LoseMenu.SetActive(true);
    }
    public void HideLose()
    {
        LoseMenu.SetActive(false);
    }
    public void HideAll()
    {
        HidePause();
        HideLose();
        HideWin();
    }
}
