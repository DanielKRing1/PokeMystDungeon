using System;
using UnityEngine;

public class CircleSpriteRenderer
{
    private SpriteRenderer spriteRenderer;

    public CircleSpriteRenderer(GameObject go)
    {
        this.spriteRenderer = go.AddComponent<SpriteRenderer>();
    }

    public void Draw(string spritePath, float radius, Color color)
    {
        // 1. Get custom sprite
        Sprite customSprite = Resources.Load<Sprite>(spritePath);

        // 2. Assign SpriteRenderer sprite and color
        this.spriteRenderer.sprite = customSprite;
        this.spriteRenderer.color = color;

        // 3. Adjust SpriteRenderer radius
        this.spriteRenderer.transform.localScale = new Vector3(radius, radius, radius);
    }
}
