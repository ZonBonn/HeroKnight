using UnityEngine;

public class BossHealthHandler : MonoBehaviour
{
    private HealthSystem healthSystem;
    private BossHealthBar bossHealthBar;

    private void Awake()
    {
        healthSystem = new HealthSystem(100f);
    }

    public void DamageBoss(float damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public void HealBoss(float healAmount)
    {
        healthSystem.Heal(healAmount);
    }

    public HealthSystem getBossHealthSystem()
    {
        return healthSystem;
    }
}
