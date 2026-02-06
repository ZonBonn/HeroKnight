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

    private void Start()
    {
        // healthSystem.OnTriggerHealthBarChange += TriggerChangeHealthBarUI; // sai vì cái này thuộc về khi SetActive qua màn last level=> OnEnable chứ không phải Start
    }

    private void OnEnable()
    {
        healthSystem.OnDamaged += TriggerChangeHealthBarUI;
        healthSystem.OnHealed += TriggerChangeHealthBarUI;
    }

    public void SetUp(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;
    }

    public void TriggerChangeHealthBarUI()
    {
        healthImage.fillAmount = healthSystem.GetHealthNormalized();
    }
}
