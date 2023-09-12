using UnityEngine;

public class CircleLineRenderer
{
    private LineRenderer lineRenderer;

    public CircleLineRenderer(GameObject go)
    {
        this.lineRenderer = go.AddComponent<LineRenderer>();
        this.lineRenderer.useWorldSpace = false;
    }

    public void Draw(
        int vertexCount,
        float width,
        float yPos,
        float radius,
        Color startColor,
        Color endColor
    )
    {
        // 1. Set color
        this.lineRenderer.startColor = startColor;
        this.lineRenderer.endColor = endColor;

        // 2. Set width
        this.lineRenderer.startWidth = width;
        this.lineRenderer.endWidth = width;

        // 3. Set vertices
        Vector3[] circleOutlineVertices = CreateOutlineVertices(vertexCount, yPos, radius);
        this.lineRenderer.positionCount = vertexCount;
        this.lineRenderer.SetPositions(circleOutlineVertices);
    }

    private Vector3[] CreateOutlineVertices(int vertexCount, float yPos, float radius)
    {
        Vector3[] vertices = new Vector3[vertexCount];

        float angle;
        float x;
        float z;
        for (int i = 0; i < vertexCount; i++)
        {
            angle = (float)i / (vertexCount - 1) * 360f;
            x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            z = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
            vertices[i] = new Vector3(x, yPos, z);
        }

        return vertices;
    }
}
