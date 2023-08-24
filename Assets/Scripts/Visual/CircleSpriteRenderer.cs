using UnityEngine;

public class CircleSpriteRenderer
{
    private SpriteRenderer spriteRenderer;

    public CircleSpriteRenderer(GameObject go)
    {
        this.spriteRenderer = go.AddComponent<SpriteRenderer>();
    }

    public void Draw(float radius, Color color)
    {
        this.spriteRenderer.color = color;
        
        this.spriteRenderer.transform.localScale = new Vector3(radius, spriteRenderer.transform.localScale.y, radius);
    }
}
