using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TransparentCircleController : MonoBehaviour
{
    public float circleRadius = 75f; //

    private Material mat;
    private Canvas canvas;

    void Start()
    {
        Image img = GetComponent<Image>();
        mat = Instantiate(img.material);
        img.material = mat;

        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        // 获取屏幕分辨率
        Vector2 screenResolution = new Vector2(Screen.width, Screen.height);
        mat.SetVector("_ScreenResolution", new Vector4(screenResolution.x, screenResolution.y, 0, 0));

        // 获取鼠标位置
        Vector2 mousePosition = Input.mousePosition;
        mat.SetVector("_CirclePosition", new Vector4(mousePosition.x, mousePosition.y, 0, 0));

        // 设置圆形半径
        mat.SetFloat("_CircleRadius", circleRadius);
    }
}