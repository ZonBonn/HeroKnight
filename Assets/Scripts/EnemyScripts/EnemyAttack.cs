using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private float minDamageAttack;
    private float maxDamageAttack;

    private EnemyPathFindingMovement enemyPathFindingMovement;
    private EnemyAnimation enemyAnimation;
    public PlayerHealthStaminaHandler playerHealthStaminaHandler;
    
    void Start()
    {
        enemyPathFindingMovement = gameObject.GetComponent<EnemyPathFindingMovement>();
        enemyAnimation = gameObject.GetComponent<EnemyAnimation>();
        
        enemyAnimation.OnTriggerEachFrames += TriggerCreateAttackPoint;
    }

    
    void Update()
    {
        
    }

    public void SetUp(float minDamageAttack, float maxDamageAttack)
    {
        this.minDamageAttack = minDamageAttack;
        this.maxDamageAttack = maxDamageAttack;
    }

    private void TriggerCreateAttackPoint(int idxFrame, Sprite[] sprites)// hàm này được gọi ở fame thứ 4 (tính từ 0) của enemy
    {
        Vector3 EnemyPosition = gameObject.transform.position;
        if(sprites == enemyAnimation.AttackSprites && idxFrame == 4)
        {
            const float attackDistance = 0.7f;
            int dirVisual = enemyPathFindingMovement.currentVisualDir;
            Vector3 attackPosition = new Vector3(EnemyPosition.x + dirVisual * attackDistance, EnemyPosition.y, EnemyPosition.z);
            // Debug.Log(attackPosition);
            bool IsHitedPlayer = IsPlayerInAttackPoint(attackPosition);
            if(IsHitedPlayer == true)
            {
                // Debug.Log("Damage Player: " + UnityEngine.Random.Range(45, 50));
                // damage player in here
                // playerHealthHandler.Damage(UnityEngine.Random.Range(45, 50));
                playerHealthStaminaHandler.DamageHealth(UnityEngine.Random.Range(minDamageAttack, maxDamageAttack));
            }
        }
        
    }

    private bool IsPlayerInAttackPoint(Vector3 attackPosition)
    {
        Vector3 PlayerPosition = Player.Instance.GetPlayerPosition();
        Vector3 EnemyPosition = gameObject.transform.position;

        // distance handler
        float distanceBtwEnemyAndAttackPosition = Vector3.Distance(attackPosition, EnemyPosition);
        float distanceBtwPlayerAndEnemy = Vector3.Distance(PlayerPosition, EnemyPosition);
        // nếu khoảng cách người chơi tới enemy mà <= khoảng cách từ enemy tới attack position => có nghĩa là đang trong phạm vi nhận sát thương
        bool IsInRangeAttack = distanceBtwPlayerAndEnemy <= distanceBtwEnemyAndAttackPosition;

        
        //visual handler
        bool IsInVision;
        Vector3 EnemyDirectToPlayer = PlayerPosition-EnemyPosition;
        float EnemyAngleVisualDirectToPlayer = Mathf.Atan2(EnemyDirectToPlayer.y, EnemyDirectToPlayer.x) * Mathf.Rad2Deg; // góc được tạo bởi trục Ox và Vector hướng từ góc nhìn enemy tới player
        int currentEnemyVisual = enemyPathFindingMovement.currentVisualDir;
        // đem so nó liệu có đang thuộc vào góc nhìn của enemy không ?
        if(currentEnemyVisual == +1)
        {
            IsInVision = -70 <= EnemyAngleVisualDirectToPlayer &&  EnemyAngleVisualDirectToPlayer <= 70;
        }
        else // currentEnemyVisual == -1 or currentEnemyVisual == 0
        {
            IsInVision = -110 >= EnemyAngleVisualDirectToPlayer || EnemyAngleVisualDirectToPlayer >= 110;
        }
        // Debug.Log("EnemyAngleVisualDirectToPlayer:"+EnemyAngleVisualDirectToPlayer);
        // Debug.Log("IsInRangeAttack: " + IsInRangeAttack + "        IsInVision: " + IsInVision);
        return IsInRangeAttack && IsInVision;
    }
}
