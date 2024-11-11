using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AimController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    public GameObject pauseMenu;

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
            if (IsPointerOverPauseButton())
            {
                TogglePauseMenu();
            }
            else
            {
                CheckNPCCollision();
            }
        }

    }

    private bool IsPointerOverPauseButton()
    {
        // 检测鼠标是否点击在 UI 元素上
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // 遍历射线检测的所有结果
            foreach (var result in results)
            {
                if (result.gameObject.name  == "PauseButton") // 假设你的暂停按钮打了 PauseButton 标签
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void TogglePauseMenu()
    {
        if (pauseMenu != null)
        {
            bool isActive = pauseMenu.activeSelf;
            pauseMenu.SetActive(!isActive);
        }
    }

    private void CheckNPCCollision()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 从鼠标位置发射一条射线，检测是否点击到 NPC
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("NPC"));
        Debug.Log("check");
        if (hit.collider != null)
        {
            var npcController = hit.collider.gameObject.GetComponent<NPCController>();
            if (npcController != null && IsSpriteVisible(hit.collider.gameObject, mousePosition))
            {
                npcController.Kill();
            }
        }
    }

    private bool IsSpriteVisible(GameObject target, Vector2 mouseWorldPosition)
    {
        // 使用射线检测可见性，确保没有其它物体遮挡
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity);

        return hit.collider != null && hit.collider.gameObject == target;
    }

}