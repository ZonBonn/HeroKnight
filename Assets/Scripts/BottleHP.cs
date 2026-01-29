using UnityEngine;

public class BottleHP : MonoBehaviour
{
    private bool canPickUpBottleHP;

    private void Awake()
    {
        canPickUpBottleHP = false;
        FunctionTimer.Create(setCanPickUpBottleHPTrue, 1f); // đếm ngược thời gian có thể nhặt tránh nhặt luôn, ngay trong file cho tiện chứ không phải tham chiếu làng nhằng như Key
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            PlayerHealthStaminaHandler playerHealthStaminaHandler = collision.GetComponent<PlayerHealthStaminaHandler>();
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if(playerHealthStaminaHandler != null && 
            playerHealthStaminaHandler.GetCurrentPlayerHealth() < playerHealthStaminaHandler.GetMaxHealth() &&
            playerMovement.GetPlayerState() != State.Die &&
            canPickUpBottleHP == true
            )
            {
                playerHealthStaminaHandler.HealHealth(50);
                Destroy(gameObject);
            }
        }
    }

    private void setCanPickUpBottleHPTrue()
    {
        canPickUpBottleHP = true;
    }
}
