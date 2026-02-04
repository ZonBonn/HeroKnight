using System;
using UnityEngine;

public class BossLevelCombatManager : MonoBehaviour
{
    private bool isBossDie;
    private bool canMakeDamage;

    private BossHealthHandler bossHealthHandler;
    private HealthSystem bossHealthSystem;

    public SupportorLevelCombatManager supportorDieStateManager1;
    public SupportorLevelCombatManager supportorDieStateManager2;

    public Action OnTriggerBossDie;

    private void Awake()
    {
        bossHealthHandler = gameObject.GetComponent<BossHealthHandler>();
    }

    private void Start()
    {
        bossHealthSystem = bossHealthHandler.GetHealthSystem();
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
        return supportorDieStateManager1.getIsSupportorDie() && supportorDieStateManager2.getIsSupportorDie();
    }
}
