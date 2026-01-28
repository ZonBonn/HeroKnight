using System;
using UnityEngine;

public class BossLevelCombatManager : MonoBehaviour
{
    private bool isBossDie;
    private bool canMakeDamage;

    private HealthHandler bossHealthHandler;
    private HealthSystem bossHealthSystem;

    public SupportorLevelCombatManager supporterDieStateManager1;
    public SupportorLevelCombatManager supporterDieStateManager2;

    public Action OnTriggerBossDie;

    private void Awake()
    {
        bossHealthHandler = gameObject.GetComponent<HealthHandler>();
        bossHealthSystem = bossHealthHandler.GetHealthSystem();
    }

    private void Start()
    {
        bossHealthSystem.OnTriggerHealthBarAsZero += OnBossDie;
    }

    public bool getIsBossDead()
    {
        return isBossDie;
    }

    public void OnBossDie() // boss hết máu thông báo cho supportor phải chết
    {
        isBossDie = true;
        // hàm thông báo cho Supportor tại đây
        OnTriggerBossDie?.Invoke();

    }

    public bool getCanMakeDamage()
    {
        canMakeDamage = checkCanMakeDamage();
        return canMakeDamage;
    }

    private bool checkCanMakeDamage()
    {
        return supporterDieStateManager1.getIsSupporterDie() && supporterDieStateManager2.getIsSupporterDie();
    }
}
