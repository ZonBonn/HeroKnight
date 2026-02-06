using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private float minDamageAttack;
    private float maxDamageAttack;

    private BossPathFindingMovement bossPathFindingMovement;
    private BossAnimation bossAnimation;
    // public PlayerHealthStaminaHandler playerHealthStaminaHandler;
    // private PlayerDefense playerDefense;
    
    void Start()
    {
        bossPathFindingMovement = gameObject.GetComponent<BossPathFindingMovement>();
        bossAnimation = gameObject.GetComponent<BossAnimation>();
        
        bossAnimation.OnTriggerEachFrames += TriggerCreateAttackPoint;

        // tham chiếu cho level 2
        // if(playerHealthStaminaHandler == null)
        // {
        //     playerHealthStaminaHandler = PlayerManager.Instance.GetPlayerGameObject().GetComponent<PlayerHealthStaminaHandler>();
        //     playerDefense = playerHealthStaminaHandler.gameObject.GetComponent<PlayerDefense>();
        // }
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
        Vector3 EnemyPosition = BossPositionHolder.Instance.GetRealBossPosition();
        if(sprites == bossAnimation.AttackSprites && idxFrame == 4)
        {
            const float attackDistance = 2.5f;
            int dirVisual = bossPathFindingMovement.currentVisualDir;
            Vector3 attackPosition = new Vector3(EnemyPosition.x + dirVisual * attackDistance, EnemyPosition.y, EnemyPosition.z);
            // Debug.Log("Real Enemy Position Center: " + EnemyPosition);
            // Debug.Log("attackPosition: " + attackPosition);
            bool IsHitedPlayer = IsPlayerInAttackPoint(attackPosition);
            if(IsHitedPlayer == true)
            {
                // Debug.Log("Hited Player");
                // Debug.Log("Damage Player: " + UnityEngine.Random.Range(45, 50));
                // damage player in here
                // playerHealthHandler.Damage(UnityEngine.Random.Range(45, 50));
                // playerHealthStaminaHandler.DamageHealth(UnityEngine.Random.Range(minDamageAttack, maxDamageAttack));
                // playerDefense.ReceiveDamage(minDamageAttack, maxDamageAttack, bossPathFindingMovement.currentVisualDir);

                // NEW dùng interface cho sạch
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.attackerDir = bossPathFindingMovement.currentVisualDir;
                damageInfo.minDamage = minDamageAttack;
                damageInfo.maxDamage = maxDamageAttack;
                damageInfo.layerMask = gameObject.layer;
                Collider2D[] hitedCollider = Physics2D.OverlapCircleAll(attackPosition, 0.1f);
                for(int i = 0 ; i < hitedCollider.Length ; i++)
                {
                    IDamageable damageable = hitedCollider[i].gameObject.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.Damage(damageInfo);  
                    }
                }
            }
        }
        
    }

    private bool IsPlayerInAttackPoint(Vector3 attackPosition)
    {
        Vector3 PlayerPosition = Player.Instance.GetPlayerPosition();
        Vector3 EnemyPosition = BossPositionHolder.Instance.GetRealBossPosition();

        // distance handler
        float distanceBtwEnemyAndAttackPosition = Vector3.Distance(attackPosition, EnemyPosition);
        float distanceBtwPlayerAndEnemy = Vector3.Distance(PlayerPosition, EnemyPosition);
        // nếu khoảng cách người chơi tới enemy mà <= khoảng cách từ enemy tới attack position => có nghĩa là đang trong phạm vi nhận sát thương
        bool IsInRangeAttack = distanceBtwPlayerAndEnemy <= distanceBtwEnemyAndAttackPosition;

        
        //visual handler
        bool IsInVision;
        Vector3 EnemyDirectToPlayer = PlayerPosition-EnemyPosition;
        float EnemyAngleVisualDirectToPlayer = Mathf.Atan2(EnemyDirectToPlayer.y, EnemyDirectToPlayer.x) * Mathf.Rad2Deg; // góc được tạo bởi trục Ox và Vector hướng từ góc nhìn enemy tới player
        int currentEnemyVisual = bossPathFindingMovement.currentVisualDir;
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
