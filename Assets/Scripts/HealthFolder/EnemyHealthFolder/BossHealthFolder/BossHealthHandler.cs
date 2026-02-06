using UnityEngine;
using System;

public class BossHealthHandler : MonoBehaviour, IHealthSystemProvider, IDamageable, IHealable
{
    private HealthSystem healthSystem;
    private BossHealthBar bossHealthBar;
    private BossLevelCombatManager bossLevelCombatManager;
    // private float 

    private void Awake()
    {
        healthSystem = new HealthSystem(100f);

        // shoud in awake() ???
        bossHealthBar = UICanvasManager.Instance.getBossHealthBar(); // không cùng scene thì phải chịu cách này thôi
        bossHealthBar.SetUp(healthSystem);

        bossLevelCombatManager  = gameObject.GetComponent<BossLevelCombatManager>();
    }

    private void Start()
    {
        // Debug.Log("Boss HealthSystem instance: " + healthSystem.GetHashCode());
    }

    public void DamageBoss(float damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public void HealBoss(float healAmount)
    {
        healthSystem.Heal(healAmount);
    }

    public void Damage(float damageAmount) // tự xử lý Damage theo con boss này, mấy thằng ligh, ew, heavy tương tự vậy
    {
        if(bossLevelCombatManager.getCanMakeDamage() == true)
        {
            DamageBoss(damageAmount);
        }
        else
        {
            DamageBoss(0);
        }
    }

    public void Heal(float healAmount)
    {
        healthSystem.Heal(healAmount);
    }

    public void RegenHPBoss()
    {
        
    }

    public HealthSystem getBossHealthSystem(){ return healthSystem; }
    
    public BossHealthBar getBossHealthBar(){ return bossHealthBar; }

    public float GetHP(){ return healthSystem.GetCurrentHealth(); }

    

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }
}
