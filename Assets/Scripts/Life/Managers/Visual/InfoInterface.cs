using UnityEngine;

public class InfoInterface : MonoBehaviour
{
    GameObject levelText;
    GameObject healthBar;
    GameObject expBar;

    void Awake()
    {
        this.levelText = this.transform.Find("LevelText").gameObject;
        this.healthBar = this.transform.Find("HealthBar").gameObject;
        this.expBar = this.transform.Find("ExpBar").gameObject;
    }

    public void UpdateLevelText(int level)
    {
        TextMesh[] texts = this.levelText.GetComponentsInChildren<TextMesh>();
        foreach (TextMesh t in texts)
        {
            t.text = "Lvl " + level;
        }
    }

    public void UpdateHealthBar(float curHealth, float maxHealth)
    {
        UnityEngine.UI.Slider slider = this.healthBar.GetComponent<UnityEngine.UI.Slider>();
        slider.value = curHealth / maxHealth;
    }

    public void UpdateExpBar(int curExp, int neededExp)
    {
        UnityEngine.UI.Slider slider = this.expBar.GetComponent<UnityEngine.UI.Slider>();
        slider.value = curExp / neededExp;
    }
}
