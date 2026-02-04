using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private float minDamageAttack;
    private float maxDamageAttack;

    private EnemyPathFindingMovement enemyPathFindingMovement;
    private EnemyAnimation enemyAnimation;
    public PlayerHealthStaminaHandler playerHealthStaminaHandler;
    private PlayerDefense playerDefense; // =))) truy cập ngược tới gameObject = playerHealth =))) (không nên lạm dụng nếu không muốn các scripts phụ thuộc nhau sai 1 thì đi 1 dàn đều sai nếu sau này sửa)


    private void Awake()
    {
        enemyPathFindingMovement = gameObject.GetComponent<EnemyPathFindingMovement>();
        enemyAnimation = gameObject.GetComponent<EnemyAnimation>();
    }
    
    void Start()
    {
        enemyAnimation.OnTriggerEachFrames += TriggerCreateAttackPoint;

        // tham chiếu cho level 2
        if(playerHealthStaminaHandler == null)
        {
            playerHealthStaminaHandler = PlayerManager.Instance.GetPlayerGameObject().GetComponent<PlayerHealthStaminaHandler>();
        }

        playerDefense = playerHealthStaminaHandler.gameObject.GetComponent<PlayerDefense>(); // cái này phải sau playerHealthStaminaHandler vì cái này phụ thuộc playerHealthStaminaHandler mà cái playerHealthStaminaHandler khởi tạo khi level được reload thì mởi khởi tạo được PlayerDefense
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
            const float attackDistance = 0.8f;
            int dirVisual = enemyPathFindingMovement.currentVisualDir;
            Vector3 attackPosition = new Vector3(EnemyPosition.x + dirVisual * attackDistance, EnemyPosition.y, EnemyPosition.z);
            // Debug.Log(attackPosition);
            bool IsHitedPlayer = IsPlayerInAttackPoint(attackPosition);
            // Debug.Log("IsHitedPlayer" + IsHitedPlayer + " IsPlayerSameDirVar" + IsPlayerSameDirVar);    
            if(IsHitedPlayer == true)
            {
                // playerHealthStaminaHandler.DamageHealth(UnityEngine.Random.Range(minDamageAttack, maxDamageAttack));
                playerDefense.ReceiveDamage(minDamageAttack, maxDamageAttack, enemyPathFindingMovement.currentVisualDir);
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
