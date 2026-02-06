using System;
using UnityEngine;

public class HealthHandler : MonoBehaviour, IHealthSystemProvider, IDamageable, IHealable // class thuộc về nhân vật sở hữu HealthBar (cầu nối để khởi tạo healthSystem)
{ 
    // Health System
    private HealthSystem healthSystem = new HealthSystem(100f);
    private HealthBar healthBar;
    private Transform healthBarTransform;

    // prefab Health Bar
    public Transform pfHealthBar;

    // health bar spwan position
    private Transform healthBarPositionTransform;

    // private for debunging
    private float currentHealth;

    private Enemy enemy;
    private float minDamageReceived;
    private float maxDamageReceived;

    private void Awake()
    {
        if (!enabled) return;

        healthBarPositionTransform = gameObject.transform.Find("HealthBarPosition").transform;

        healthBarTransform = Instantiate(pfHealthBar, healthBarPositionTransform.position, Quaternion.identity, healthBarPositionTransform);
        
        healthBar = healthBarTransform.GetComponent<HealthBar>();
        healthBar.SetUp(healthSystem);

        enemy.gameObject.GetComponent<Enemy>();
    }

    private void Start()
    {
        enemy.getFeature(out float minDamageReceived, out float maxDamageReceived, out float minDamageAttack, out float maxDamageAttack);
        this.minDamageReceived = minDamageReceived;
        this.maxDamageReceived = maxDamageReceived;
    }
    
    public void Damage(float damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public void Heal(float healAmount)
    {
        healthSystem.Heal(healAmount);
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }

    public HealthBar GetHealthBar()
    {
        return healthBar;
    }

    // for testing bug
    private void Update()
    {
        currentHealth = healthSystem.GetCurrentHealth();
    }

    public float GetHP()
    {
        return healthSystem.GetCurrentHealth();
    }

    // ============== INTERFACE ==============
    public void Damage(DamageInfo damageInfo)
    {
        if(damageInfo.layerMask == gameObject.layer) return; // tránh không cho cùng Layer vã lẫn nhau
        
        float finalDamage = caculateFinalDamage(damageInfo);
        Damage(finalDamage);
    }

    public float caculateFinalDamage(DamageInfo damageInfo)
    {
        if(damageInfo.attackType == AttackType.Attack1)
        {
            return minDamageReceived;
        }
        else if(damageInfo.attackType == AttackType.Attack2)
        {
            return maxDamageReceived;
        }
        else if(damageInfo.attackType == AttackType.Attack3)
        {
            return maxDamageReceived + 5;
        }
        return 0;
    }
}
