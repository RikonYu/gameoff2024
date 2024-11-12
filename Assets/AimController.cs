using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AimController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

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

        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIElement() && IsPointerOverSpecificUIElement(pauseButton.gameObject))
                {
                    pauseMenu.SetActive(true);
                }
            else
            {
                HandleSceneClick();
            }
        }

    }

    bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
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


    void HandleSceneClick()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0f; // Adjust Z to match sprite positions

        // Find all SpriteRenderers at the mouse position
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();
        List<SpriteRenderer> spritesUnderMouse = new List<SpriteRenderer>();

        Debug.Log("check");

        foreach (SpriteRenderer sr in allSprites)
        {
            if (sr.bounds.Contains(worldPoint))
            {
                spritesUnderMouse.Add(sr);
            }
        }

        if (spritesUnderMouse.Count > 0)
        {
            // Sort sprites from topmost to bottommost
            spritesUnderMouse.Sort((a, b) =>
            {
                int layerOrderA = SortingLayer.GetLayerValueFromID(a.sortingLayerID);
                int layerOrderB = SortingLayer.GetLayerValueFromID(b.sortingLayerID);

                if (layerOrderA != layerOrderB)
                    return layerOrderB.CompareTo(layerOrderA);

                if (a.sortingOrder != b.sortingOrder)
                    return b.sortingOrder.CompareTo(a.sortingOrder);

                // Lower Z means closer to the camera in 2D
                return b.transform.position.z.CompareTo(a.transform.position.z);
            });

            SpriteRenderer topSprite = spritesUnderMouse[0];
            NPCController npc = topSprite.GetComponent<NPCController>();

            if (npc != null)
            {
                npc.Kill();
                Debug.Log("NPC killed");
            }
        }
    }
}