using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Obstacle : MonoBehaviour
{
    public bool isWalkable = true;

    public ObstacleData GetData()
    {
        ObstacleData data = new ObstacleData();
        data.position = transform.position;
        data.scale = transform.localScale;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            data.spriteName = sr.sprite.name;
        }

        data.isWalkable = isWalkable;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            data.colliderType = collider.GetType().Name;
            data.colliderParameters = new ColliderParameters();
            data.colliderParameters.offset = collider.offset;

            if (collider is BoxCollider2D boxCollider)
            {
                data.colliderParameters.size = boxCollider.size;
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                data.colliderParameters.radius = circleCollider.radius;
            }
            else if (collider is PolygonCollider2D polygonCollider)
            {
                data.colliderParameters.points = new List<Vector2>(polygonCollider.points);
            }
            else if (collider is EdgeCollider2D edgeCollider)
            {
                data.colliderParameters.edgePoints = new List<Vector2>(edgeCollider.points);
            }
            else
            {
                Debug.LogWarning($"Unsupported collider type: {collider.GetType().Name}");
            }
        }

        return data;
    }

    public static void CreateFromData(ObstacleData data)
    {
        GameObject obj = new GameObject("Obstacle");
        obj.layer = LayerMask.NameToLayer("Obstacles");
        Obstacle obstacle = obj.AddComponent<Obstacle>();
        obj.transform.position = data.position;
        obj.transform.localScale = data.scale;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        Sprite sprite = Resources.Load<Sprite>($"Sprites/{data.spriteName}");
        if (sprite != null)
        {
            sr.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Sprite '{data.spriteName}' not found in Resources/Sprites.");
        }

        obstacle.isWalkable = data.isWalkable;

        if (!string.IsNullOrEmpty(data.colliderType))
        {
            Collider2D collider = null;

            if (data.colliderType == nameof(BoxCollider2D))
            {
                BoxCollider2D boxCollider = obj.AddComponent<BoxCollider2D>();
                boxCollider.size = data.colliderParameters.size;
                boxCollider.offset = data.colliderParameters.offset;
                collider = boxCollider;
            }
            else if (data.colliderType == nameof(CircleCollider2D))
            {
                CircleCollider2D circleCollider = obj.AddComponent<CircleCollider2D>();
                circleCollider.radius = data.colliderParameters.radius;
                circleCollider.offset = data.colliderParameters.offset;
                collider = circleCollider;
            }
            else if (data.colliderType == nameof(PolygonCollider2D))
            {
                PolygonCollider2D polygonCollider = obj.AddComponent<PolygonCollider2D>();
                polygonCollider.points = data.colliderParameters.points.ToArray();
                polygonCollider.offset = data.colliderParameters.offset;
                collider = polygonCollider;
            }
            else if (data.colliderType == nameof(EdgeCollider2D))
            {
                EdgeCollider2D edgeCollider = obj.AddComponent<EdgeCollider2D>();
                edgeCollider.points = data.colliderParameters.edgePoints.ToArray();
                edgeCollider.offset = data.colliderParameters.offset;
                collider = edgeCollider;
            }
            else
            {
                Debug.LogWarning($"Unsupported collider type: {data.colliderType}");
            }
        }
    }
}