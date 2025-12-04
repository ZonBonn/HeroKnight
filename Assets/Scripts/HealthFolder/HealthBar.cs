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
        healthHandler.OnTriggerHealthBarChange += TriggerHealthBarChange;
    }

    private void TriggerHealthBarChange()
    {
        Bar.localScale = new Vector2(healthHandler.GetHealthNormalized(healthSystem.currentHealth), 1f);
    }
}
