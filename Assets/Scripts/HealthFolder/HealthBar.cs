using UnityEngine;
using System;

public class HealthBar : MonoBehaviour // class này quản lý UI của HealthBar trong game
{
    private HealthHandler healthHandler;
    private HealthSystem healthSystem;

    private Transform Bar;

    private void Start()
    {
        healthHandler = gameObject.GetComponent<HealthHandler>();
        healthSystem = healthHandler.GetHealthSystem();
        Bar = gameObject.transform.Find("Bar");
        healthSystem.OnTriggerHealthBarChange += TriggerHealthBarChange;
    }

    private void TriggerHealthBarChange()
    {
        Bar.localScale = new Vector2(healthSystem.GetHealthNormalized(healthSystem.GetCurrentHealth()), 1f);
    }

    public void SetUp(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;
    }
}
