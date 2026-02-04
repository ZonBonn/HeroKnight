using UnityEngine;
using System;

public class BossHealthHandler : MonoBehaviour, IHealthSystemProvider
{
    private HealthSystem healthSystem;
    private BossHealthBar bossHealthBar;

    private void Awake()
    {
        healthSystem = new HealthSystem(100f);

        // shoud in awake() ???
        bossHealthBar = UICanvasManager.Instance.getBossHealthBar(); // không cùng scene thì phải chịu cách này thôi
        bossHealthBar.SetUp(healthSystem);
    }

    private void Start()
    {
        Debug.Log("Boss HealthSystem instance: " + healthSystem.GetHashCode());
    }

    public void DamageBoss(float damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public void HealBoss(float healAmount)
    {
        healthSystem.Heal(healAmount);
    }

    public HealthSystem getBossHealthSystem(){ return healthSystem; }
    
    public BossHealthBar getBossHealthBar(){ return bossHealthBar; }

    public float GetHP(){ return healthSystem.GetCurrentHealth(); }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }
}
