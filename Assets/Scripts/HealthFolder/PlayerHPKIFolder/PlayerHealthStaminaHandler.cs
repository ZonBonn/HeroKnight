using UnityEngine;
using System;

public class PlayerHealthStaminaHandler : MonoBehaviour // class này quản lý HP và KI của nhân vật (cầu nối để khởi tạo hai class không thừa kế monobehaviour)
{
    private PlayerHealthSystem playerHealthSystem = new PlayerHealthSystem(100f);
    private PlayerStaminaSystem playerStaminaSystem = new  PlayerStaminaSystem(100f);

    public PlayerHealthBar playerHealthBar;
    public PlayerStaminaBar playerStaminaBar;

    public PlayerMovement playerMovement;

    public Action OnBlockIdleIsHited;

    private void Awake()
    {
        // playerHealthBar = gameObject.GetComponent<PlayerHealthBar>();
        // playerHealthBar.playerHealthSystem = playerHealthSystem; // làm như này thì playerHealthSystem không được encapsulation
        playerHealthBar.SetUp(playerHealthSystem);

        // playerStaminaBar = gameObject.GetComponent<PlayerStaminaBar>();
        // playerStaminaBar.playerStaminaSystem = playerStaminaSystem; // làm như này thì playerStaminaSystem không được encapsulation
        playerStaminaBar.SetUp(playerStaminaSystem);
    }

    private void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    public void DamageHealth(float damageAmount)
    {
        

        if(playerMovement.GetPlayerState() == State.BlockIdle) // đỡ được thì sao ?
        {
            OnBlockIdleIsHited?.Invoke();
        }
        else
        {
            playerHealthSystem.Damage(damageAmount);
        }
    }

    public void HealHealth(float healAmount)
    {
        playerHealthSystem.Heal(healAmount);
    }

    private void DamageStamina(float damageAmount)
    {
        playerStaminaSystem.Damage(damageAmount);
    }

    public void HealStamina(float healAmount)
    {
        playerStaminaSystem.Heal(healAmount);
    }

    public bool TryToUseStamina(float manaCost)
    {
        float currentStamina = playerStaminaSystem.GetCurrentStamina();
        if(manaCost > currentStamina)
        {
            return false;
        }
        DamageStamina(manaCost);
        return true;
    }


    public PlayerHealthSystem GetPlayerHealthSystem()
    {
        return playerHealthSystem;
    }

    public PlayerStaminaSystem GetPlayerStaminaSystem()
    {
        return playerStaminaSystem;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P) && Input.GetKeyDown(KeyCode.H))
        {
            HealHealth(100);
        }
        if (Input.GetKeyDown(KeyCode.P) && Input.GetKeyDown(KeyCode.S))
        {
            DamageHealth(100);
        }
        #endif
    }
}
