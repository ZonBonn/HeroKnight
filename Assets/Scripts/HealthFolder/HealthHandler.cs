using System;
using UnityEngine;

public class HealthHandler : MonoBehaviour // class thuộc về nhân vật sở hữu HealthBar
{
    public Action OnTriggerHealthBarChange;
    public Action OnTriggerHealthBarAsZero;
    HealthSystem healthSystem = new HealthSystem();

    private void Start()
    {
        healthSystem.SetUp(100);
    }

    public void Damage(float damageAmount)
    {
        healthSystem.currentHealth -= damageAmount; 
        if(healthSystem.currentHealth < 0)
        {
            healthSystem.currentHealth = 0;
            // trigger
            OnTriggerHealthBarAsZero.Invoke();
        }
        // trigger
        OnTriggerHealthBarChange.Invoke();
    }

    public void Heal(float healAmount)
    {
        healthSystem.currentHealth += healAmount; 
        if(healthSystem.currentHealth > healthSystem.maxHealth)
        {
            healthSystem.currentHealth = healthSystem.maxHealth;
        }

        //trigger
        OnTriggerHealthBarChange.Invoke();
    }

    public float GetHealthNormalized(float currentHealth)
    {
        return currentHealth / healthSystem.maxHealth;
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }
}
