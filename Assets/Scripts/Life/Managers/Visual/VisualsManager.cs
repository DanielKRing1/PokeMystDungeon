using UnityEngine;

public class VisualsManager : MonoBehaviour
{
    private bool isInitted = false;

    private CircleLineRenderer VisionOutline;
    private CircleSpriteRenderer LeaderIndicator;

    // Start is called before the first frame update
    public void Init()
    {
        if (this.isInitted)
            return;
        this.isInitted = true;

        // 1. Add Vision Outline
        this.InitVisionOutline();

        // 2. Add Leader Indicator
        this.InitLeadershipIndicator();

        // 3. Add Info: HealthBar + Level Info
        this.InitInfoBar();

        // 4. Apply Root Leader color
        // CANNOT DO THIS, NEED TO INITIALIZE LEADERSHIPMANAGER FIRST
        // BUT LEVELMANAGER NEEDS THIS INITIALIZED
        // this.ApplyRootLeaderColor();
    }

    private void InitVisionOutline()
    {
        GameObject visionOutlineGO = this.CreateVisualsChild(
            "VisionOutline",
            new Vector3(0, -0.45f, 0),
            Quaternion.Euler(Vector3.zero)
        );
        this.VisionOutline = new CircleLineRenderer(visionOutlineGO);
    }

    private void InitLeadershipIndicator()
    {
        GameObject leaderIndicatorGO = this.CreateVisualsChild(
            "LeaderIndicator",
            new Vector3(0, -0.45f, 0),
            Quaternion.Euler(new Vector3(90f, 0, 0))
        );
        this.LeaderIndicator = new CircleSpriteRenderer(leaderIndicatorGO);
    }

    private void InitInfoBar()
    {
        GameObject infoBarPrefab = Resources.Load("Prefabs/Info") as GameObject;
        GameObject infoBarGO = Instantiate(infoBarPrefab, Vector3.zero, Quaternion.identity);
        infoBarGO.name = "Info";
        this.SetParent(infoBarGO, new Vector3(0, 1.5f, 0), Quaternion.identity);
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
