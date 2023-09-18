using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoInterface : MonoBehaviour
{
    GameObject healthBar;
    GameObject levelText;

    void Awake()
    {
        this.healthBar = this.transform.Find("HealthBar").gameObject;
        this.levelText = this.transform.Find("LevelText").gameObject;
    }

    public void UpdateHealthBar(float curHealth, float maxHealth)
    {
        UnityEngine.UI.Slider slider = this.healthBar.GetComponent<UnityEngine.UI.Slider>();
        slider.value = curHealth / maxHealth;
    }

    public void UpdateLevelText(int level)
    {
        TextMesh[] texts = this.levelText.GetComponentsInChildren<TextMesh>();
        foreach (TextMesh t in texts)
        {
            t.text = "Lvl " + level;
        }
    }
}
