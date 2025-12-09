using System;
using UnityEngine;

public class PlayerStaminaSystem // class này lưu giữ KI thật
{
    private float currentStamina;
    private float maxStamina;
    private float staminaRegenAmount;

    public Action OnTriggerPlayerStaminaChange;
    public Action OnTriggerPlayerStaminaAsZero;

    public PlayerStaminaSystem(float maxStamina)
    {
        currentStamina = maxStamina;
        this.maxStamina = maxStamina;
        staminaRegenAmount = 30f;
    }

    public void Damage(float damageAmount)
    {
        currentStamina -= damageAmount;
        if(currentStamina <= 0)
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

    public void RegenStamina()
    {
        if(currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
            return;
        }
        currentStamina += staminaRegenAmount * Time.deltaTime;
        
        OnTriggerPlayerStaminaChange?.Invoke();
    }

    public float GetStaminaNormalized()
    {
        return currentStamina / maxStamina;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public void Update()
    {
        RegenStamina();
    }
}
