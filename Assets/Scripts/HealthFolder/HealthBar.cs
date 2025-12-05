using UnityEngine;
using System;

public class HealthBar : MonoBehaviour // class này quản lý UI của HealthBar trong game
{
    private HealthSystem healthSystem;

    private Transform Bar;

    private void Start()
    {
        Bar = gameObject.transform.Find("Heal").transform;
        healthSystem.OnTriggerHealthBarChange += TriggerHealthBarChange;
    }

    private void TriggerHealthBarChange()
    {
        Bar.localScale = new Vector2(healthSystem.GetHealthNormalized(), 1f);
    }

    public void SetUp(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;
    }
}
