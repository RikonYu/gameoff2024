using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

public class AimController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    public Tilemap tilemap;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    public AudioSource shootSound, reloadSound;
    public static AimController instance;
    void Start()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    float shootCooldown = 0f;
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 anchoredPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out anchoredPosition);

        rectTransform.anchoredPosition = anchoredPosition;

        shootCooldown -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            if (shootCooldown > 0f)
            {

                return;
            }

            StartCoroutine(shootandreload());

            shootCooldown = Consts.ShootCooldown;
            if (IsPointerOverUIElement() && IsPointerOverSpecificUIElement(pauseButton.gameObject))
                {
                GameController.instance.pauseGame();
                    pauseMenu.SetActive(true);
                }
            else
            {
                DetectTilesAtMousePosition();
            }
        }

    }

    bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private System.Collections.IEnumerator shootandreload()
    {
        shootSound.Play();

        while (shootSound.isPlaying)
        {
            yield return null;
        }
        reloadSound.Play();
    }

    bool IsPointerOverSpecificUIElement(GameObject target)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == target)
            {
                return true;
            }
        }
        return false;
    }


    void DetectTilesAtMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseWorldPos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos2D);


        var hits = Physics2D.RaycastAll(mouseWorldPos, Vector3.forward, 20f);

        Array.Sort(hits, (a, b) => { return a.collider.gameObject.transform.position.y.CompareTo(b.collider.gameObject.transform.position.y); });



        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<Building>() != null)
                break;
            if (hit.collider.gameObject.transform.parent.GetComponent<NPCController>() != null)
                hit.collider.gameObject.transform.parent.GetComponent<NPCController>().Kill();

            
        }

    }
}