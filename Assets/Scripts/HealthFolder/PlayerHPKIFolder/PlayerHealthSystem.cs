using System;

public class PlayerHealthSystem // class này lưu giữ HP thật
{
    private float currentHealth;
    private float maxHealth;

    public Action OnTriggerPlayerHealthChange;
    public Action OnTriggerPlayerHealthAsZero;

    public PlayerHealthSystem(float maxHealth)
    {
        currentHealth = maxHealth;
        this.maxHealth = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if(currentHealth < 0)
        {
            currentHealth = 0;

            OnTriggerPlayerHealthChange?.Invoke();
        }

        OnTriggerPlayerHealthChange?.Invoke();
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        OnTriggerPlayerHealthChange?.Invoke();
    }

    public float GetHealthNormalized()
    {
        return currentHealth / maxHealth;
    }
}
