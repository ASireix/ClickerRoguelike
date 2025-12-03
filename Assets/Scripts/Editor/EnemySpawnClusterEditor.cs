using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawnCluster))]
public class EnemySpawnClusterEditor : Editor
{
    private const string previewName = "__Preview";

    void OnSceneGUI()
    {
        EnemySpawnCluster cluster = (EnemySpawnCluster)target;

        // Draw disc in Scene view
        Handles.color = new Color(1f, 0f, 0f, 0.25f);
        Handles.DrawSolidDisc(cluster.transform.position, Vector3.forward, cluster.radius);

        Handles.color = Color.red;
        Handles.DrawWireDisc(cluster.transform.position, Vector3.forward, cluster.radius);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CreateOrUpdatePreview();
    }

    private void CreateOrUpdatePreview()
    {
        EnemySpawnCluster cluster = (EnemySpawnCluster)target;

        // Find or create preview object
        Transform preview = cluster.transform.Find(previewName);

        if (preview == null)
        {
            GameObject go = new GameObject(previewName);
            go.transform.SetParent(cluster.transform);
            go.transform.localPosition = Vector3.zero;
            preview = go.transform;

            // Add sprite renderer
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite();
            sr.color = new Color(1f, 0f, 0f, 0.15f);
            sr.sortingOrder = 9999;
        }

        // Update scale
        float diameter = cluster.radius * 2f;
        preview.localScale = new Vector3(diameter, diameter, 1f);
    }

    private Sprite CreateCircleSprite()
    {
        // Create a simple 16x16 red circle texture
        Texture2D tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        Color transparent = new Color(1, 0, 0, 0);

        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                Vector2 p = new Vector2(x - 8, y - 8);
                if (p.magnitude <= 8)
                    tex.SetPixel(x, y, Color.white);
                else
                    tex.SetPixel(x, y, transparent);
            }
        }

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
    }
}
