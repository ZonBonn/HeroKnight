using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour // class này nhiệm vụ hiển thị UI theo biến currentcurrentStamina của PlayerStaminaSystem
{
    private Transform Bar;

    private Image staminaImage;
    
    private PlayerStaminaSystem playerStaminaSystem;

    private void Start()
    {
        Bar = gameObject.transform.Find("Bar");
        staminaImage = Bar.GetComponent<Image>();
    }

    private void Update()
    {
        playerStaminaSystem.Update();
    }

    public void SetUp(PlayerStaminaSystem playerStaminaSystem)
    {
        this.playerStaminaSystem = playerStaminaSystem;
        playerStaminaSystem.OnTriggerPlayerStaminaChange += TriggerStaminaBarChange;
    }

    private void TriggerStaminaBarChange()
    {
        staminaImage.fillAmount = playerStaminaSystem.GetStaminaNormalized();
    }


}
