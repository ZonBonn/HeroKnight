using System;
using UnityEngine;

public class HealthSystem // đây là nơi thực sự chứa hp
{
    private float currentHealth;
    private float maxHealth;

    public Action OnHealed;
    public Action OnDamaged; // khi damage tên này không chuẩn (vì BossAI bị Hurt khi máu dù tăng trong Heal ?)
    public Action OnTriggerHealthBarAsZero;

    public HealthSystem(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount; 
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            // Debug.Log("OnTriggerHealthBarAsZero Invoked");
            OnTriggerHealthBarAsZero?.Invoke();
        }
        // OnTriggerHealthBarChange?.Invoke();
        OnDamaged?.Invoke();
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount; 
        if(currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealed?.Invoke();
    }

    public float GetHealthNormalized()
    {
        return currentHealth / maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
