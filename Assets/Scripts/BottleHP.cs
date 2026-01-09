using UnityEngine;

public class BottleHP : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            PlayerHealthStaminaHandler playerHealthStaminaHandler = collision.GetComponent<PlayerHealthStaminaHandler>();
            if(playerHealthStaminaHandler != null && playerHealthStaminaHandler.GetCurrentPlayerHealth() < playerHealthStaminaHandler.GetMaxHealth())
            {
                playerHealthStaminaHandler.HealHealth(50);
                Destroy(gameObject);
            }
        }
    }
}
