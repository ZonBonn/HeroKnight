using System;

public class PlayerStaminaSystem // class này lưu giữ KI thật
{
    private float currentStamina;
    private float maxStamina;

    public Action OnTriggerPlayerStaminaChange;
    public Action OnTriggerPlayerStaminaAsZero;

    public PlayerStaminaSystem(float maxStamina)
    {
        currentStamina = maxStamina;
        this.maxStamina = maxStamina;
    }

    public void Damage(float damageAmount)
    {
        currentStamina -= damageAmount;
        if(currentStamina < 0)
        {
            currentStamina = 0;
            
            OnTriggerPlayerStaminaAsZero?.Invoke();
        }
        OnTriggerPlayerStaminaChange?.Invoke();
    }

    public void Heal(float healAmount)
    {
        currentStamina += healAmount;
        if(currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        OnTriggerPlayerStaminaChange?.Invoke();
    }

    public float GetStaminaNormalized()
    {
        return currentStamina / maxStamina;
    }
}
