using UnityEngine;

public class VisualsManager : MonoBehaviour
{
    private CircleLineRenderer VisionOutline;
    private CircleSpriteRenderer LeaderIndicator;

    // Start is called before the first frame update
    public void Start()
    {
        // Vision Outline
        GameObject visionOutlineGO = this.CreateVisualsChild(
            "VisionOutline",
            new Vector3(0, -0.45f, 0),
            Quaternion.Euler(Vector3.zero)
        );
        this.VisionOutline = new CircleLineRenderer(visionOutlineGO);

        // Leader Indicator
        GameObject leaderIndicatorGO = this.CreateVisualsChild(
            "LeaderIndicator",
            new Vector3(0, -0.45f, 0),
            Quaternion.Euler(new Vector3(90f, 0, 0))
        );
        this.LeaderIndicator = new CircleSpriteRenderer(leaderIndicatorGO);

        // HealthBar
        GameObject healthBarPrefab = Resources.Load("Prefabs/HealthBar") as GameObject;
        GameObject healthBarGO = Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity);
        healthBarGO.name = "HealthBar";
        this.SetParent(healthBarGO, new Vector3(0, 1.5f, 0), Quaternion.identity);

        this.ApplyRootLeaderColor();
    }

    private GameObject CreateVisualsChild(
        string name,
        Vector3 relativePosition,
        Quaternion relativeRotation
    )
    {
        // 1. Create child go
        GameObject visualsChild = new GameObject(name);
        this.SetParent(visualsChild, relativePosition, relativeRotation);

        return visualsChild;
    }

    private void SetParent(GameObject go, Vector3 relativePosition, Quaternion relativeRotation)
    {
        // 1. Set parent/make child
        go.transform.SetParent(this.gameObject.transform);

        // 2. Set position
        go.transform.localPosition = relativePosition;
        // 3. Set rotation
        go.transform.localRotation = relativeRotation;
    }

    public void ApplyRootLeaderColor()
    {
        Color color = this.GetComponent<LeadershipManager>().GetRootLeaderColor();

        this.VisionOutline.Draw(
            20,
            0.25f,
            0f,
            this.GetComponent<StatsManager>().GetStats().Vision,
            color,
            color
        );
        this.LeaderIndicator.Draw("Sprites/RadialCircle", 0.05f, color);
    }
}
