using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    private Transform Bar;

    private Image staminaImage;
    
    private PlayerStaminaSystem playerStaminaSystem;

    private void Start()
    {
        Bar = gameObject.transform.Find("Bar");
        staminaImage = Bar.GetComponent<Image>();

        playerStaminaSystem.OnTriggerPlayerStaminaChange += TriggerStaminaBarChange;
    }

    public void SetUp(PlayerStaminaSystem playerStaminaSystem)
    {
        this.playerStaminaSystem = playerStaminaSystem;
    }

    private void TriggerStaminaBarChange()
    {
        staminaImage.fillAmount = playerStaminaSystem.GetStaminaNormalized();
    }


}
