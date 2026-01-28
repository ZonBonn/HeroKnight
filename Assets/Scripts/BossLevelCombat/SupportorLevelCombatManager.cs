using System;
using System.Linq.Expressions;
using NUnit.Framework;
using UnityEngine;

public class SupportorLevelCombatManager : MonoBehaviour
{
    private HealthHandler supportorHealthHandler;
    private HealthSystem supportorhealthSystem;
    public Action OnTriggerReviveSupportor;

    public GameObject bossGameObject; // reference Unity
    private BossLevelCombatManager bossLevelCombatManager;
    // private HealthHandler bossHealthHandler;
    // private HealthSystem bossHealthSystem;


    private bool isSupportorDie;
    
    private void Awake()
    {
        supportorHealthHandler = gameObject.GetComponent<HealthHandler>();
        supportorhealthSystem = supportorHealthHandler.GetHealthSystem();
        OnTriggerReviveSupportor += OnSupportorRevive;
    }

    private void Start()
    {
        supportorhealthSystem.OnTriggerHealthBarAsZero += OnSupportorDie;
        bossLevelCombatManager = bossGameObject.GetComponent<BossLevelCombatManager>();
        // bossLevelCombatManager.OnTriggerBossDie += OnSupportorDie;
        bossLevelCombatManager.OnTriggerBossDie += TriggerDieWhenBossDie; // khi boss tạch thì supportor cũng phải tạch
    }

    public bool getIsSupportorDie()
    {
        return isSupportorDie;
    }

    public void OnSupportorDie() // khi supportor tạch thì hàm này sẽ được chạy bởi
    {
        isSupportorDie = true;
        Debug.Log("Thực hiện hồi sinh Supportor sau 10s");
        FunctionTimer.Create(OnTriggerReviveSupportor, 10f); // đăng ký action rồi cho hàm đăng ký chỉ việc chạy Action để chạy các publisher
    }

    private void OnSupportorRevive() // chỉnh trong này cho encapsulation
    {
        // nếu boss mà chưa chết thì mới có thể chuyển trạng thái từ chết => sống
        if(bossGameObject == null) return;
        if (bossGameObject.GetComponent<BossLevelCombatManager>().getIsBossDead() == false) 
        {
            isSupportorDie = false;
        }
    }

    public void TriggerDieWhenBossDie()
    {
        isSupportorDie = true;
        gameObject.GetComponent<EnemyEWAI>().OnTriggerWhenBossDie?.Invoke();
    }
}
