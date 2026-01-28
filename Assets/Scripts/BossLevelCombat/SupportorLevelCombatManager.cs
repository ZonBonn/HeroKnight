using System;
using System.Linq.Expressions;
using NUnit.Framework;
using UnityEngine;

public class SupportorLevelCombatManager : MonoBehaviour
{
    private HealthHandler supporterHealthHandler;
    private HealthSystem supporterhealthSystem;
    public Action OnTriggerReciveSupportor;

    public GameObject bossGameObject; // reference Unity
    private BossLevelCombatManager bossLevelCombatManager;
    // private HealthHandler bossHealthHandler;
    // private HealthSystem bossHealthSystem;


    private bool isSupporterDie;
    
    private void Awake()
    {
        supporterHealthHandler = gameObject.GetComponent<HealthHandler>();
        supporterhealthSystem = supporterHealthHandler.GetHealthSystem();
    }

    private void Start()
    {
        supporterhealthSystem.OnTriggerHealthBarAsZero += OnSupporterDie;
        
        // boss Health System Health Handler
        // bossHealthHandler = bossGameObject.GetComponent<HealthHandler>();
        // bossHealthSystem = bossHealthHandler.GetHealthSystem();
        // bossHealthSystem.OnTriggerHealthBarAsZero += OnTriggerBossDie; // boss hết máu thông báo cho supportor phải chết
        bossLevelCombatManager = bossGameObject.GetComponent<BossLevelCombatManager>();
        bossLevelCombatManager.OnTriggerBossDie += OnSupporterDie; // khi boss tạch thì supportor cũng phải tạch
    }

    public bool getIsSupporterDie()
    {
        return isSupporterDie;
    }

    public void OnSupporterDie()
    {
        isSupporterDie = true;
        Debug.Log("Thực hiện hồi sinh Supportor sau 10s");
        FunctionTimer.Create(OnTriggerReciveSupportor, 10f); // đăng ký action rồi cho hàm đăng ký chỉ việc chạy Action để chạy các publisher
    }
}
