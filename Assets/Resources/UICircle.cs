using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UICircle : Graphic
{
    [Range(3, 360)]
    public int segments = 60; // 圆形的精细程度

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        float deltaAngle = 2 * Mathf.PI / segments;
        float radius = rectTransform.rect.width / 2;

        // 添加中心点
        vh.AddVert(Vector3.zero, color, Vector2.zero);

        // 添加周围的顶点
        for (int i = 0; i <= segments; i++)
        {
            float angle = deltaAngle * i;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            vh.AddVert(new Vector3(x, y), color, Vector2.zero);
        }

        // 添加三角形
        for (int i = 1; i <= segments; i++)
        {
            vh.AddTriangle(0, i, i + 1);
        }
    }
}
