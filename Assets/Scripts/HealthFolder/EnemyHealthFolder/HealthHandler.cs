using System;
using UnityEngine;

public class HealthHandler : MonoBehaviour // class thuộc về nhân vật sở hữu HealthBar (cầu nối để khởi tạo healthSystem)
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

    private void Awake()
    {
        healthBarPositionTransform = gameObject.transform.Find("HealthBarPosition").transform;

        healthBarTransform = Instantiate(pfHealthBar, healthBarPositionTransform.position, Quaternion.identity, healthBarPositionTransform);
        
        healthBar = healthBarTransform.GetComponent<HealthBar>();
        healthBar.SetUp(healthSystem);
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

    // for testing bug
    private void Update()
    {
        currentHealth = healthSystem.GetCurrentHealth();
    }
}
