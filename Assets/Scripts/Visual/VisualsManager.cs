using UnityEngine;

public class VisualsManager : MonoBehaviour
{
    private CircleLineRenderer VisionOutline;
    private CircleSpriteRenderer LeaderIndicator;

    // Start is called before the first frame update
    public void Start()
    {
        GameObject visionOutlineGO = this.CreateVisualsChild(
            "VisionOutline",
            new Vector3(0, -0.45f, 0),
            Quaternion.Euler(Vector3.zero)
        );
        this.VisionOutline = new CircleLineRenderer(visionOutlineGO);

        GameObject leaderIndicatorGO = this.CreateVisualsChild(
            "LeaderIndicator",
            new Vector3(0, -0.45f, 0),
            Quaternion.Euler(new Vector3(90f, 0, 0))
        );
        this.LeaderIndicator = new CircleSpriteRenderer(leaderIndicatorGO);

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
        // 2. Set parent/make child
        visualsChild.transform.SetParent(this.gameObject.transform);

        // 3. Set position
        visualsChild.transform.localPosition = relativePosition;
        // 4. Set rotation
        visualsChild.transform.localRotation = relativeRotation;

        return visualsChild;
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
