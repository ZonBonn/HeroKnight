using UnityEngine;
using System;
using System.Security.Cryptography;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyStateAction { Patrol, Chase, Attack, ReadyToAttack, Idle, Die, Jump, Hurt, Recovery };
    public EnemyStateAction currentEnemyStateAction;
    private EnemyStateAction lastCheckedCurrentEnemyStateAction;
    private EnemyPathFindingMovement enemyPathFindingMovement = null;
    public Transform currentToward;
    private Transform lastCheckedCurrentToward;
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform chaseLeftPoint;
    public Transform chaseRightPoint;
    private Transform nullTransform = null; // đây sẽ là vị trí mà currentToward hướng tới khi đang trong các trạng thái khác ngoài state Patrol // khi bị Hurt Interupt là không thể vì khi đến gần thì nó tự chuyển thành RTC mà RTC sẽ đổi thanh nullTransform luôn rồi
    private Enemy enemy;
    private EnemyAnimation enemyAnimation;
    public LayerMask playerLayerMask;
    private float idleTimer;
    private float m_IdleTimer; // nếu biến này bị Hurt interupt mà không reset có bị sao không nhỉ ? (lười mò để check quá :D, check sau tại đây, nếu có lỗi liên quan tới Idle và RTC)
    private float rTCTimer = 0.5f;
    private float m_RTCTimer; // thời gian chờ ready to combat để chuyển thành attack // nếu biến này bị Hurt interupt mà không reset có bị sao không nhỉ ?  (lười mò để check quá :D, check sau tại đây nếu có lỗi liên quan tới Idle và RTC)

    private bool IsPlayerAround;
    private Vector3 PlayerPosition;
    private Vector3 EnemyPosition;
    private float DistanceEnemyToPlayer;

    // public PlayerHealthStaminaHandler playerHealthStaminaHandler; // ???

    public bool IsHurting; // ????

    private HealthHandler enemyHealthHandler;
    private HealthSystem enemyHealthSystem;
    private HealthBar enemyHealthBar;

    private bool isJumping;
    private bool isDied = false;

    public PlayerMovement playerMovement;
    private EnemySensor enemySensor;

    private const float READY_TO_COMBAT_COOLDOWN = 0.5f;
    private const float PATROL_REACHED_DISTANCE = 0.6f; // ngưỡng xác nhận đã tới vị trí tuần tra
    private const float READY_TO_ATTACK_DISTANCE = 1.5f; // ngưỡng enemy sẽ vào trạng thái chuẩn bị tấn công
    private const float ATTACK_DISTANCE = 1f; // ngưỡng mà enemy sẽ tấn công
    private const float  DISENGAGE_DISTANCE = 4f; // ngưỡng mà enemy quyết định còn đuổi hay không đuổi tiếp ??? nó như là MAX_CHASE vậy
    private const float CHASE_MIN_DISTANCE = 2f; // ngưỡng mà enemy quyết định còn đuổi hay không đuổi tiếp ??? nó như là MIN_CHASE vậy

    
    private void Awake()
    {
        enemyPathFindingMovement = gameObject.GetComponent<EnemyPathFindingMovement>();
        enemy = gameObject.GetComponent<Enemy>();
        enemyAnimation = gameObject.GetComponent<EnemyAnimation>();
        enemyHealthHandler = gameObject.GetComponent<HealthHandler>();

        enemySensor = gameObject.GetComponent<EnemySensor>();
    }

    private void Start()
    {
        currentEnemyStateAction = EnemyStateAction.Patrol;
        currentToward = UnityEngine.Random.Range(-1, 1) > 0 ? rightPoint : leftPoint;
        lastCheckedCurrentToward = currentToward == rightPoint ? leftPoint : rightPoint;

        enemyHealthSystem = enemyHealthHandler.GetHealthSystem();
        enemyHealthBar = enemyHealthHandler.GetHealthBar();

        enemyAnimation.OnTriggerEachFrames += TriggerEnemyLastAttackFrameHandler;
        enemyAnimation.OnTriggerEachFrames += TriggerEnemyLastJumpFrameHandler;
        // enemyAnimation.OnTriggerEachFrames += TriggerCreateAttackPoint;
        enemyAnimation.OnTriggerLastFrames += TriggerEnemyLastHurtFrameHandler;
        enemyAnimation.OnTriggerLastFrames += TriggerEnemyLastDieFrameHandler;
        enemyAnimation.OnTriggerLastFrames += TriggerEnemyLastRecoveryFrame;

        enemyHealthSystem.OnTriggerHealthBarChange += TriggerHurtWhenHealthChange;
        enemyHealthSystem.OnTriggerHealthBarAsZero += TriggerDieWhenHealthAsZero;

        idleTimer = UnityEngine.Random.Range(2.5f, 3f);

        // tham chiếu cho level 2
        if(playerMovement == null)
        {
            playerMovement = PlayerManager.Instance.GetPlayerGameObject().GetComponent<PlayerMovement>();
        }
    }

    private void Update()
    {
        if(isDied == true)
        {
            // recovery enemy in here ????

            return;
        }
        

        IsPlayerAround = enemySensor.IsSearchedPlayerAround();
        PlayerPosition = Player.Instance.GetPlayerPosition();
        EnemyPosition = gameObject.transform.position;
        DistanceEnemyToPlayer = Vector3.Distance(PlayerPosition, EnemyPosition);

        switch (currentEnemyStateAction)
        {
            case EnemyStateAction.Patrol:
                enemyAnimation.AnimationHandler(EnemyState.Run);
                PatrolActionHandler();
                break;
            case EnemyStateAction.Idle:
                enemyAnimation.AnimationHandler(EnemyState.Idle);
                IdleActionHandler();
                break;
            case EnemyStateAction.Chase:
                enemyAnimation.AnimationHandler(EnemyState.Run);
                ChaseActionHandler();
                break;
            case EnemyStateAction.Attack:
                enemyAnimation.AnimationHandler(EnemyState.Attack);
                AttackActionHandler();
                break;
            case EnemyStateAction.ReadyToAttack:
                enemyAnimation.AnimationHandler(EnemyState.ReadyToCombat);
                ReadyToAttackActionHandler();
                break;
            case EnemyStateAction.Jump:
                enemyAnimation.AnimationHandler(EnemyState.Jump);
                JumpActionHandler();
                break;
            case EnemyStateAction.Hurt:
                enemyAnimation.AnimationHandler(EnemyState.Hurt);
                HurtActionHandler();
                break;
            case EnemyStateAction.Die:
                enemyAnimation.AnimationHandler(EnemyState.Death);
                DeathActionHandler();
                break;
            case EnemyStateAction.Recovery:
                enemyAnimation.AnimationHandler(EnemyState.Recovery);
                RecoveryActionHandler();
                break;
        }

        // enemyAnimation.AnimationHandler(currentEnemyStateAction); // dòng này có cũng được không có cũng được vì case nào sẽ chạy animation đó rồi mà

        if (lastCheckedCurrentEnemyStateAction != currentEnemyStateAction)
        {
            // Debug.Log(currentEnemyStateAction);
            lastCheckedCurrentEnemyStateAction = currentEnemyStateAction;
        }
    }
    
    // ========== STATE ENEMY HANDLERS ==========
    private void PatrolActionHandler()
    {
        Immediately_m_RTCTimer();

        if (currentToward != lastCheckedCurrentToward)
        {
            if(currentToward == nullTransform)
            {
                enemyPathFindingMovement.MoveTo(lastCheckedCurrentToward.position);
                currentToward = lastCheckedCurrentToward;
            }
            else
            {
                enemyPathFindingMovement.MoveTo(currentToward.position);
                lastCheckedCurrentToward = currentToward;
            }
        }

        if (Vector3.Distance(EnemyPosition, currentToward.position) <= PATROL_REACHED_DISTANCE)
        {
            currentEnemyStateAction = EnemyStateAction.Idle;
            m_IdleTimer = idleTimer;
            // Debug.Log("Đổi hướng");
            // Debug.Log("Đổi thanh Idle");
            if (currentToward == leftPoint)
            {
                currentToward = rightPoint;
            }
            else
            {
                currentToward = leftPoint;
            }
            enemyPathFindingMovement.StopMovingPhysicalHandler();
        }
        
        if(IsPlayerAround) // alway finds player in here <-- SearchingPlayerAround();
        {
            currentToward = nullTransform; // đổi hướng khi chuyển trạng thái cho đỡ phải tốn chi phí, lại còn tạo ra random hướng của mỗi enemy
            currentEnemyStateAction = EnemyStateAction.Chase;
            return;
        }
        
        if (enemyPathFindingMovement.IsHole() || /*Input.GetKeyDown(KeyCode.L) ||*/ enemyPathFindingMovement.IfCanJumpOverTheInFrontWall())
        {
            currentToward = nullTransform;
            // Debug.Log("Is Hole:" + enemyPathFindingMovement.IsHole() + "  ;;;   IfCanJumpOverTheInFrontWall:" + enemyPathFindingMovement.IfCanJumpOverTheInFrontWall());
            currentEnemyStateAction = EnemyStateAction.Jump;
            isJumping = true;
            return;
        }
        ReadyToAttackImmediately();
    }

    private void IdleActionHandler()
    {
        // if (enemyPathFindingMovement.IsHole() || Input.GetKeyDown(KeyCode.L)) // tmp
        // {
        //     currentEnemyStateAction = EnemyStateAction.Jump;
        //     isJumping = true;
        //     return;
        // }
        enemyPathFindingMovement.StopMovingPhysicalHandler();
        Immediately_m_RTCTimer();
        if (m_IdleTimer > 0)
        {
            m_IdleTimer -= Time.deltaTime;
        }
        else
        {
            currentEnemyStateAction = EnemyStateAction.Patrol;
            // Debug.Log("Đổi thanh Patrol");
            return;
        }
        if(IsPlayerAround)// alway finds player in here <-- SearchingPlayerAround();
        {
            currentEnemyStateAction = EnemyStateAction.Chase;
            return;
        }
        ReadyToAttackImmediately();
    }

    private void ChaseActionHandler()
    {
        enemySensor.AlwayTowardToPlayer();
        Immediately_m_RTCTimer();
        if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true)
        {
            currentEnemyStateAction = EnemyStateAction.ReadyToAttack;
            return;
        }
        else
        {
            enemyPathFindingMovement.MoveTo(PlayerPosition);
            // trong lúc đang đuổi theo nên check hố chướng ngại vật các thứ v.v tại đây --> IN HERE <-- tại đây
        }

        if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
        {
            currentEnemyStateAction = EnemyStateAction.Patrol;
            return;
        }
        if (enemyPathFindingMovement.IsHole() || /*Input.GetKeyDown(KeyCode.L) ||*/ enemyPathFindingMovement.IfCanJumpOverTheInFrontWall())
        {
            currentEnemyStateAction = EnemyStateAction.Jump;
            isJumping = true;
            return;
        }
    }

    private void AttackActionHandler()
    {
        enemyPathFindingMovement.StopMovingPhysicalHandler();
        if (m_RTCTimer > 0) // đợi chờ chờ tới lượt đánh tiếp theo
        {
            currentEnemyStateAction = EnemyStateAction.ReadyToAttack;
            return;
        }    
        if (IsPlayerAround == false && DistanceEnemyToPlayer <= ATTACK_DISTANCE)
        {
            // while attacking player and dont see the player
            enemyPathFindingMovement.MoveTo(PlayerPosition);
        }
    }

    private void ReadyToAttackActionHandler()
    {
        m_RTCTimer -= Time.deltaTime;
        // viết hàm luôn luôn nhìn về hướng player khi đang ở trạng thái readyTOAttack tại đây
        enemySensor.AlwayTowardToPlayer();
        enemyPathFindingMovement.StopMovingPhysicalHandler();
        if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && m_RTCTimer <= 0)
        {
            currentEnemyStateAction = EnemyStateAction.Attack;
            return;
        }
        if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
        {
            currentEnemyStateAction = EnemyStateAction.Patrol;
            return;
        }
        if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
        IsPlayerAround == true && 
        DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
        {
            currentEnemyStateAction = EnemyStateAction.Chase;
            return;
        }
        if(playerMovement.GetPlayerState() == State.Die) // --> error in here  <--
        {
            currentEnemyStateAction = EnemyStateAction.Patrol;
            return;
        }
        // Debug.Log("rơi vào nhánh này khi không rơi vào bất kì nhánh nào của RTA");
    }

    private void JumpActionHandler()
    {
        if(enemyPathFindingMovement.IsGrounded() == true && isJumping == false) // đã tiếp đất thì mới được chuyển trạng thái 
        {
            if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
            {
                currentEnemyStateAction = EnemyStateAction.Patrol;
                return;
            }
            if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true)
            {
                currentEnemyStateAction = EnemyStateAction.ReadyToAttack;
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                currentEnemyStateAction = EnemyStateAction.Chase;
                return;
            }
            currentEnemyStateAction = EnemyStateAction.Chase;
            return;
        }
        else // chưa tiếp đất mà đang trên không
        {
            // do nothing
        }
    }
    
    private void HurtActionHandler()
    {
        // do nothing or do not change the state during hurting
    }
    
    private void DeathActionHandler()
    {
        // do nothing or do not change the state during Dying
    }        
    
    private void RecoveryActionHandler()
    {
        // do nothing or do not change the state during Recovering
    }
    // =============================================================


    // =============== HANDLER ENEMY ANIMATION BY EACH FRAMES ===============
    private void TriggerEnemyLastAttackFrameHandler(int idxFrame, Sprite[] sprites)
    {
        if (sprites == enemyAnimation.AttackSprites && idxFrame == enemyAnimation.AttackSprites.Length - 1)
        {
            m_RTCTimer = rTCTimer;
        }
    }
    
    private void TriggerEnemyLastJumpFrameHandler(int idxFrame, Sprite[] sprites)
    {
        if(sprites == enemyAnimation.JumpSprites && idxFrame == enemyAnimation.JumpSprites.Length)
        {
            isJumping = false;
        }
    }
    
    private void TriggerEnemyFirstHurtFrameHandler(Sprite[] sprites)
    {
        if(sprites == enemyAnimation.HurtSprites)
        {
            
        }
    }
    
    private void TriggerEnemyLastHurtFrameHandler(Sprite[] sprites)
    {
        
        if(sprites == enemyAnimation.HurtSprites)
        {
            bool IsPlayerAround = enemySensor.IsSearchedPlayerAround();
            if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true && m_RTCTimer <= 0)
            {
                currentEnemyStateAction = EnemyStateAction.Attack;
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                currentEnemyStateAction = EnemyStateAction.Chase;
                return;
            }

            // Debug.Log("K nhảy vào state nào cả thì nhảy vào patrol");
            currentEnemyStateAction = EnemyStateAction.Patrol; // safe state
            return;
            
        }
    }

    private void TriggerEnemyLastDieFrameHandler(Sprite[] sprites)
    {
        if(sprites == enemyAnimation.DeathSprites)
        {
            isDied = true;
        }
    }
    
    private void TriggerEnemyLastRecoveryFrame(Sprite[] sprites)
    {
        if(sprites == enemyAnimation.RecoverSprites)
        {
            currentEnemyStateAction = EnemyStateAction.Idle;
        }
    }
    // ===========================================================
    

    // =============== HANDLER ENEMY HEALTH BY EACH FRAMES ===============
    private void TriggerHurtWhenHealthChange()
    {
        if (enemyHealthSystem.GetCurrentHealth() <= 0)
        {
            // HP = 0 thì KHÔNG BAO GIỜ vào state Hurt
            return;
        }
        if(currentEnemyStateAction == EnemyStateAction.Hurt)
        {
            return;
        }
        currentEnemyStateAction = EnemyStateAction.Hurt;
    }
    
    private void TriggerDieWhenHealthAsZero()
    {
        if(currentEnemyStateAction == EnemyStateAction.Die)
        {
            return;
        }
        currentEnemyStateAction = EnemyStateAction.Die;

        // set something when enemy is died
        enemyPathFindingMovement.StopMovingPhysicalHandler();
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        enemyHealthBar.gameObject.SetActive(false);
    }
    // ===========================================================

    // ========= SUPPORTING FUNCTION ========
    private void Immediately_m_RTCTimer()
    {
        m_RTCTimer = 0;
    }

    // private void AlwayTowardToPlayer(){
    //     Vector3 PlayerPosition = Player.Instance.GetPlayerPosition();
    //     Vector3 EnemyPosition = gameObject.transform.position;
    //     Vector2 DirToTarget = PlayerPosition - EnemyPosition;
                
    //     // handler Visual Direction and Flip Direction
    //     float tmpDirToTargetX = DirToTarget.x;
    //     if (-0.55f <= tmpDirToTargetX && tmpDirToTargetX <= 0.55f)
    //     {
    //         tmpDirToTargetX = enemyPathFindingMovement.currentVisualDir; // default value if the amount too small
    //     }
        
    //     int moveDirX = Math.Sign(tmpDirToTargetX);
        
    //     enemyPathFindingMovement.currentVisualDir = enemyPathFindingMovement.LeftOrRightPlatformer(moveDirX) == true ? 1 : -1;
    // }

    // private bool IsSearchedPlayerAround() 
    // {
    //     Vector3 EnemyPosition = gameObject.transform.position;
    //     Vector2 VisualDir = enemyPathFindingMovement.currentVisualDir == -1 ? Vector2.left : Vector2.right;
    //     Vector3 maxDistanceVisualPoint = VisualDir == Vector2.left ? chaseLeftPoint.position : chaseRightPoint.position;
    //     // đây sẽ là hai đểm ChaseWaypointA hoặc ChaseWaypointB
    //     float DistanceVisual = Vector3.Distance(EnemyPosition, maxDistanceVisualPoint);
    //     RaycastHit2D raycastHit2D = Physics2D.Raycast(EnemyPosition, VisualDir, DistanceVisual, playerLayerMask);
    //     Debug.DrawLine(EnemyPosition, maxDistanceVisualPoint, Color.darkBlue, 0.1f);
    //     if (raycastHit2D.collider != null)
    //     {
    //         return true;
    //     }
    //     return false;
    // }
    
    // private void TriggerCreateAttackPoint(int idxFrame, Sprite[] sprites)// hàm này được gọi ở fame thứ 4 (tính từ 0) của enemy
    // {
    //     Vector3 EnemyPosition = gameObject.transform.position;
    //     if(sprites == enemyAnimation.AttackSprites && idxFrame == 4)
    //     {
    //         const float attackDistance = 0.7f;
    //         int dirVisual = enemyPathFindingMovement.currentVisualDir;
    //         Vector3 attackPosition = new Vector3(EnemyPosition.x + dirVisual * attackDistance, EnemyPosition.y, EnemyPosition.z);
    //         // Debug.Log(attackPosition);
    //         bool IsHitedPlayer = IsPlayerInAttackPoint(attackPosition);
    //         if(IsHitedPlayer == true)
    //         {
    //             // Debug.Log("Damage Player: " + UnityEngine.Random.Range(45, 50));
    //             // damage player in here
    //             // playerHealthHandler.Damage(UnityEngine.Random.Range(45, 50));
    //             playerHealthStaminaHandler.DamageHealth(UnityEngine.Random.Range(45, 50));
    //         }
    //     }
        
    // }

    // private bool IsPlayerInAttackPoint(Vector3 attackPosition)
    // {
    //     Vector3 PlayerPosition = Player.Instance.GetPlayerPosition();
    //     Vector3 EnemyPosition = gameObject.transform.position;

    //     // distance handler
    //     float distanceBtwEnemyAndAttackPosition = Vector3.Distance(attackPosition, EnemyPosition);
    //     float distanceBtwPlayerAndEnemy = Vector3.Distance(PlayerPosition, EnemyPosition);
    //     // nếu khoảng cách người chơi tới enemy mà <= khoảng cách từ enemy tới attack position => có nghĩa là đang trong phạm vi nhận sát thương
    //     bool IsInRangeAttack = distanceBtwPlayerAndEnemy <= distanceBtwEnemyAndAttackPosition;

        
    //     //visual handler
    //     bool IsInVision;
    //     Vector3 EnemyDirectToPlayer = PlayerPosition-EnemyPosition;
    //     float EnemyAngleVisualDirectToPlayer = Mathf.Atan2(EnemyDirectToPlayer.y, EnemyDirectToPlayer.x) * Mathf.Rad2Deg; // góc được tạo bởi trục Ox và Vector hướng từ góc nhìn enemy tới player
    //     int currentEnemyVisual = enemyPathFindingMovement.currentVisualDir;
    //     // đem so nó liệu có đang thuộc vào góc nhìn của enemy không ?
    //     if(currentEnemyVisual == +1)
    //     {
    //         IsInVision = -70 <= EnemyAngleVisualDirectToPlayer &&  EnemyAngleVisualDirectToPlayer <= 70;
    //     }
    //     else // currentEnemyVisual == -1 or currentEnemyVisual == 0
    //     {
    //         IsInVision = -110 >= EnemyAngleVisualDirectToPlayer || EnemyAngleVisualDirectToPlayer >= 110;
    //     }
    //     // Debug.Log("EnemyAngleVisualDirectToPlayer:"+EnemyAngleVisualDirectToPlayer);
    //     // Debug.Log("IsInRangeAttack: " + IsInRangeAttack + "        IsInVision: " + IsInVision);
    //     return IsInRangeAttack && IsInVision;
    // }
    
    private void ReadyToAttackImmediately()
    {
        if(Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) <= READY_TO_ATTACK_DISTANCE && playerMovement.GetPlayerState() != State.Die)
        {
            if(currentEnemyStateAction == EnemyStateAction.Patrol)
            {
                currentToward = nullTransform;
            }
            enemySensor.AlwayTowardToPlayer();
            currentEnemyStateAction = EnemyStateAction.ReadyToAttack;
            return;
        }
    }
    
    public void SetIsDied(bool isDied) // hàm này chỉ được tham chiếu bởi EnemySupportTestTool không được tham chiếu hàm này tới bất kì class nào khác
    {
        this.isDied = isDied;
    }
    
    public bool GetIsDied()
    {
        return isDied;
    }
    // =============================================================
    
    private void ReadyToCombatInterrupt(float interruptsTimer)
    {
        if (interruptsTimer > 0)
        {
            interruptsTimer -= Time.deltaTime;
            float newInterruptsTimer = interruptsTimer;
            ReadyToCombatInterrupt(newInterruptsTimer);
        }
        else
        {
            return;
        }
    }
}
