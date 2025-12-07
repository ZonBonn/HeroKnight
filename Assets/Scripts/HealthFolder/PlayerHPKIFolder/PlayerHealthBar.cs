using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private Transform Bar;

    private Image healthImage;

    private PlayerHealthSystem playerHealthSystem;

    private void Start()
    {
        Bar = gameObject.transform.Find("Bar");
        healthImage = Bar.GetComponent<Image>();

        playerHealthSystem.OnTriggerPlayerHealthChange += TriggerPlayerHealthBarChange;
    }

    public void SetUp(PlayerHealthSystem playerHealthSystem)
    {
        this.playerHealthSystem = playerHealthSystem;
    }

    private void TriggerPlayerHealthBarChange()
    {
        healthImage.fillAmount = playerHealthSystem.GetHealthNormalized();
    }
}   
