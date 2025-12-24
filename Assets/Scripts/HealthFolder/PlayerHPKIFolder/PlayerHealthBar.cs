using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour // class này nhiệm vụ hiển thị UI theo biến currentHealth của PlayerHealthSystem
{
    private Transform Bar;

    private Image healthImage;

    private PlayerHealthSystem playerHealthSystem;

    private void Start()
    {
        Bar = gameObject.transform.Find("Bar");
        healthImage = Bar.GetComponent<Image>();  
    }

    public void SetUp(PlayerHealthSystem playerHealthSystem)
    {
        this.playerHealthSystem = playerHealthSystem;
        playerHealthSystem.OnTriggerPlayerHealthChange += TriggerPlayerHealthBarChange;
    }

    private void TriggerPlayerHealthBarChange(float currentHealth)
    {
        healthImage.fillAmount = playerHealthSystem.GetHealthNormalized();
    }
}   
