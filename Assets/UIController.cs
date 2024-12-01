using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject WinMenu;
    public GameObject LoseMenu;
    public AudioSource losesound;
    public GameObject Stain;

    public static UIController instance;

    

    public RectTransform circleImage;


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
        losesound.Play();
        ShowLight(Vector2.zero, 1f);
        
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
        Stain.SetActive(false);
        circleImage.transform.parent.gameObject.SetActive(false);
    }

    public void ShowLight(Vector2 worldPosition, float totalDuration)
    {
        circleImage.transform.parent.gameObject.SetActive(true);
        circleImage.gameObject.SetActive(true);

        Debug.Log("showlight");
        StartCoroutine(ExpandCircle(worldPosition, totalDuration));
    }

    private IEnumerator ExpandCircle(Vector2 worldPosition, float totalDuration)
    {

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        circleImage.position = screenPosition;

        Vector2 initialSize = new Vector2(10f, 10f);
        circleImage.sizeDelta = initialSize;

        float screenDiagonal = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
        Vector2 finalSize = new Vector2(screenDiagonal, screenDiagonal);

        Debug.Log(finalSize);

        float timer = 0f;
        while (timer < totalDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalDuration);

            circleImage.sizeDelta = Vector2.Lerp(initialSize, finalSize, progress);

            yield return null;
        }
        Stain.SetActive(true);
        circleImage.sizeDelta = finalSize;
        LoseMenu.SetActive(true);
    }
}
