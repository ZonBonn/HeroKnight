using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    private Transform healthBar;
    private Image healthImage;

    private HealthSystem healthSystem;

    private void Awake()
    {
        healthBar = gameObject.transform.Find("BossHealthBar");
        healthImage = healthBar.GetComponent<Image>();
    }

    private void SetUp(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;
    }

    public void TriggerChangeHealthBarUI()
    {
        // healthImage.fillAmount = 
    }
}
