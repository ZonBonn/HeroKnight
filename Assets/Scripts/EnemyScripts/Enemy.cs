using UnityEngine;

public enum MonsterType { Ground, Sky }
public enum EnemyType { LightEnemy, HeavyEnemy, EvilWizard }
public class Enemy : MonoBehaviour
{
    public MonsterType monsterType;
    public EnemyType enemyType;

    private EnemyAttack enemyAttack;

    private void Awake()
    {
        enemyAttack = gameObject.GetComponent<EnemyAttack>();
        if(enemyType == EnemyType.LightEnemy)
        {
            enemyAttack.SetUp(30, 40);
        }
        else if(enemyType == EnemyType.HeavyEnemy)
        {
            enemyAttack.SetUp(40, 50);
        }
        else if(enemyType == EnemyType.EvilWizard)
        {
            
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
