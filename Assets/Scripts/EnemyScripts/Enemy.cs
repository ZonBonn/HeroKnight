using UnityEngine;

public enum MonsterType { Ground, Sky }
public enum EnemyType { LightEnemy, HeavyEnemy, EvilWizard, Boss }
public class Enemy : MonoBehaviour
{
    public MonsterType monsterType;
    public EnemyType enemyType;

    private EnemyAttack enemyAttack;
    private EnemyEWShooting enemyEWShooting;
    private BossAttack bossAttack;

    [SerializeField] private float minDamageReceived;
    [SerializeField] private float maxDamageReceived;

    [SerializeField] private float minDamageAttack;
    [SerializeField] private float maxDamageAttack;


    private void Awake()
    {
        enemyAttack = gameObject.GetComponent<EnemyAttack>();
        enemyEWShooting = gameObject.GetComponent<EnemyEWShooting>();
        bossAttack = gameObject.GetComponent<BossAttack>();
        
        if(enemyType == EnemyType.LightEnemy && enemyAttack != null)
        {
            enemyAttack.SetUp(minDamageAttack, maxDamageAttack);
        }
        else if(enemyType == EnemyType.HeavyEnemy && enemyAttack != null)
        {
            enemyAttack.SetUp(minDamageAttack, maxDamageAttack);
        }
        else if(enemyType == EnemyType.EvilWizard && enemyEWShooting != null)
        {
            
        }
        else if(enemyType == EnemyType.Boss && bossAttack != null)
        {
            bossAttack.SetUp(minDamageAttack, maxDamageAttack);
        }
    }

    public void getFeature(out float minDamageReceived, out float maxDamageReceived, out float minDamageAttack, out float maxDamageAttack)
    {
        minDamageReceived = this.minDamageReceived;
        maxDamageReceived = this.maxDamageReceived;
        minDamageAttack = this.minDamageAttack;
        maxDamageAttack = this.maxDamageAttack;
    }
}
