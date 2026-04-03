using UnityEngine;
using System;

public class BossHealthHandler : MonoBehaviour, IHealthSystemProvider, IDamageable, IHealable
{
    private HealthSystem healthSystem;
    private BossHealthBar bossHealthBar;
    private BossLevelCombatManager bossLevelCombatManager;
    private Enemy enemy;
    private float RegenHPAmount = 2.5f;

    private float minDamageReceived;
    private float maxDamageReceived;

    private void Awake()
    {
        healthSystem = new HealthSystem(100f);
        bossHealthBar = UICanvasManager.Instance.getBossHealthBar(); // không cùng scene thì phải tham chiếu khi load scene này
        bossHealthBar.SetUp(healthSystem);

        bossLevelCombatManager  = gameObject.GetComponent<BossLevelCombatManager>();

        enemy = gameObject.GetComponent<Enemy>();
    }

    private void Start()
    {
        // Debug.Log("Boss HealthSystem instance: " + healthSystem.GetHashCode());
        enemy.getFeature(out float minDamageReceived, out float maxDamageReceived, out float minDamageAttack, out float maxDamageAttack);
        this.minDamageReceived = minDamageReceived;
        this.maxDamageReceived = maxDamageReceived;
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
        Heal(RegenHPAmount * Time.deltaTime);
    }

    public HealthSystem getBossHealthSystem(){ return healthSystem; }
    
    public BossHealthBar getBossHealthBar(){ return bossHealthBar; }

    public float GetHP(){ return healthSystem.GetCurrentHealth(); }

    

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }

    // ============================ INTERFACE ============================
    public void Damage(DamageInfo damageInfo)
    {
        if(damageInfo.layerMask == gameObject.layer) return; // tránh không cho cùng Layer vã lẫn nhau
        
        if(bossLevelCombatManager.getCanMakeDamage() == true)
        {
            float finalDamage = caculateFinalDamage(damageInfo);
            DamageBoss(finalDamage);

            DamagePopup.Create(gameObject.transform.position, finalDamage, false); // bật hiệu ứng hiển thị HP bị mất 
        }
        else
        {
            DamagePopup.Create(gameObject.transform.position, 0, false);
            DamageBoss(0);
        }
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
