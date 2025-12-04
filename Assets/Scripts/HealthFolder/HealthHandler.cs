using System;
using UnityEngine;

public class HealthHandler : MonoBehaviour // class thuộc về nhân vật sở hữu HealthBar
{ 
    HealthSystem healthSystem = new HealthSystem();
    HealthBar healthBar;

    

    private void Start()
    {
        healthBar = gameObject.GetComponent<HealthBar>();
        healthBar.SetUp(healthSystem);
    }

    

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }
}
