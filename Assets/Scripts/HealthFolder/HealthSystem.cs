using System;

public class HealthSystem
{
    private float currentHealth;
    private float maxHealth = 100f;

    public Action OnTriggerHealthBarChange;
    public Action OnTriggerHealthBarAsZero;

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount; 
        if(currentHealth < 0)
        {
            currentHealth = 0;
            // trigger
            OnTriggerHealthBarAsZero?.Invoke();
        }
        // trigger
        OnTriggerHealthBarChange?.Invoke();
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount; 
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        //trigger
        OnTriggerHealthBarChange?.Invoke();
    }

    public float GetHealthNormalized(float currentHealth)
    {
        return currentHealth / maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
