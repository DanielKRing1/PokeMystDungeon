using UnityEngine;

public class CircleLineRenderer
{
    private int vertexCount;

    public CircleLineRenderer(int vertexCount)
    {
        this.vertexCount = vertexCount;
    }

    public void Draw(GameObject go, float radius, Color startColor, Color endColor)
    {
        LineRenderer lineRenderer = go.AddComponent<LineRenderer>();

        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
        
        Vector3[] circleOutlineVertices = CreateOutlineVertices(radius);
        lineRenderer.SetPositions(circleOutlineVertices);
    }

    private Vector3[] CreateOutlineVertices(float radius)
    {
        Vector3[] vertices = new Vector3[this.vertexCount + 1];

        for (int i = 0; i <= vertexCount; i++)
        {
            float angle = (float)i / vertexCount * 360f;
            float x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float z = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
            vertices[i] = new Vector3(x, 0f, z);
        }

        return vertices;
    }
}
