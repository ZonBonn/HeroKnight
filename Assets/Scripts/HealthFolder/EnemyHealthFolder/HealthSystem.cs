using System;

public class HealthSystem // đây là nơi thực sự chứa hp
{
    private float currentHealth;
    private float maxHealth;

    public Action OnTriggerHealthBarChange;
    public Action OnTriggerHealthBarAsZero;

    public HealthSystem(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount; 
        if(currentHealth < 0)
        {
            currentHealth = 0;

            OnTriggerHealthBarAsZero?.Invoke();
        }
        OnTriggerHealthBarChange?.Invoke();
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount; 
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnTriggerHealthBarChange?.Invoke();
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
