using System;

public class PlayerHealthSystem // class này lưu giữ HP thật
{
    private float currentHealth;
    private float maxHealth;

    public Action<float> OnTriggerPlayerHealthChange;
    public Action OnTriggerPlayerHealthAsZero;

    public PlayerHealthSystem(float maxHealth)
    {
        currentHealth = maxHealth;
        this.maxHealth = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if(currentHealth <= 0)
        {
            currentHealth = 0;

            OnTriggerPlayerHealthAsZero?.Invoke();
        }

        OnTriggerPlayerHealthChange?.Invoke(currentHealth);
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if(currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }

        OnTriggerPlayerHealthChange?.Invoke(currentHealth);
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
