using UnityEngine;

public class EnemyEWAI : MonoBehaviour
{
    public enum EnemyEWStateAction { Patrol, Chase, Attack, ReadyToAttack, Idle, Die, Jump, Hurt, Recovery };
    public EnemyEWStateAction currentEnemyStateAction;
    private EnemyEWStateAction lastCheckedCurrentEnemyStateAction;
    private EnemyEWPathFindingMovement enemyEWPathFindingMovement = null;
    public Transform currentToward;
    private Transform lastCheckedCurrentToward;
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform chaseLeftPoint;
    public Transform chaseRightPoint;
    private Transform nullTransform = null; // đây sẽ là vị trí mà currentToward hướng tới khi đang trong các trạng thái khác ngoài state Patrol // khi bị Hurt Interupt là không thể vì khi đến gần thì nó tự chuyển thành RTC mà RTC sẽ đổi thanh nullTransform luôn rồi
    private Enemy enemy;
    private EnemyEWAnimation enemyEWAnimation;
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
    private EnemyEWSensor enemyEWSensor;

    private const float READY_TO_COMBAT_COOLDOWN = 0.5f;
    private const float PATROL_REACHED_DISTANCE = 0.8f; // ngưỡng xác nhận đã tới vị trí tuần tra
    private const float READY_TO_ATTACK_DISTANCE = 1.5f; // ngưỡng enemy sẽ vào trạng thái chuẩn bị tấn công
    private const float ATTACK_DISTANCE = 1f; // ngưỡng mà enemy sẽ tấn công
    private const float  DISENGAGE_DISTANCE = 4f; // ngưỡng mà enemy quyết định còn đuổi hay không đuổi tiếp ??? nó như là MAX_CHASE vậy
    private const float CHASE_MIN_DISTANCE = 2f; // ngưỡng mà enemy quyết định còn đuổi hay không đuổi tiếp ??? nó như là MIN_CHASE vậy
    private const float ATTACK_RANGE = 7f; // phạm vi còn có thể tấn công của EW


    
    private void Awake()
    {
        enemyEWPathFindingMovement = gameObject.GetComponent<EnemyEWPathFindingMovement>();
        enemy = gameObject.GetComponent<Enemy>();
        enemyEWAnimation = gameObject.GetComponent<EnemyEWAnimation>();
        enemyHealthHandler = gameObject.GetComponent<HealthHandler>();

        enemyEWSensor = gameObject.GetComponent<EnemyEWSensor>();
    }

    private void Start()
    {
        currentEnemyStateAction = EnemyEWStateAction.Patrol;
        currentToward = UnityEngine.Random.Range(-1, 1) > 0 ? rightPoint : leftPoint;
        lastCheckedCurrentToward = currentToward == rightPoint ? leftPoint : rightPoint;

        enemyHealthSystem = enemyHealthHandler.GetHealthSystem();
        enemyHealthBar = enemyHealthHandler.GetHealthBar();

        enemyEWAnimation.OnTriggerEachFrames += TriggerEnemyLastAttackFrameHandler;
        enemyEWAnimation.OnTriggerEachFrames += TriggerEnemyLastJumpFrameHandler;
        // enemyAnimation.OnTriggerEachFrames += TriggerCreateAttackPoint;
        enemyEWAnimation.OnTriggerLastFrames += TriggerEnemyLastHurtFrameHandler;
        enemyEWAnimation.OnTriggerLastFrames += TriggerEnemyLastDieFrameHandler;
        enemyEWAnimation.OnTriggerLastFrames += TriggerEnemyLastRecoveryFrame;

        enemyHealthSystem.OnTriggerHealthBarChange += TriggerHurtWhenHealthChange;
        enemyHealthSystem.OnTriggerHealthBarAsZero += TriggerDieWhenHealthAsZero;

        idleTimer = UnityEngine.Random.Range(2.5f, 3f);
        m_IdleTimer = idleTimer;

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
        

        IsPlayerAround = enemyEWSensor.IsSearchedPlayerAround();
        PlayerPosition = Player.Instance.GetPlayerPosition();
        EnemyPosition = gameObject.transform.position;
        DistanceEnemyToPlayer = Vector3.Distance(PlayerPosition, EnemyPosition);

        switch (currentEnemyStateAction)
        {
            case EnemyEWStateAction.Patrol:
                enemyEWAnimation.AnimationHandler(EnemyEWState.Run);
                PatrolActionHandler();
                break;
            case EnemyEWStateAction.Idle:
                enemyEWAnimation.AnimationHandler(EnemyEWState.Idle);
                IdleActionHandler();
                break;
            case EnemyEWStateAction.Chase:
                enemyEWAnimation.AnimationHandler(EnemyEWState.Run);
                ChaseActionHandler();
                break;
            case EnemyEWStateAction.Attack:
                enemyEWAnimation.AnimationHandler(EnemyEWState.Attack);
                AttackActionHandler();
                break;
            // case EnemyEWStateAction.ReadyToAttack:
            //     enemyEWAnimation.AnimationHandler(EnemyEWState.ReadyToCombat);
            //     ReadyToAttackActionHandler();
            //     break;
            case EnemyEWStateAction.Jump:
                enemyEWAnimation.AnimationHandler(EnemyEWState.Jump);
                JumpActionHandler();
                break;
            case EnemyEWStateAction.Hurt:
                enemyEWAnimation.AnimationHandler(EnemyEWState.Hurt);
                HurtActionHandler();
                break;
            case EnemyEWStateAction.Die:
                enemyEWAnimation.AnimationHandler(EnemyEWState.Die);
                DeathActionHandler();
                break;
            case EnemyEWStateAction.Recovery:
                enemyEWAnimation.AnimationHandler(EnemyEWState.Recovery);
                RecoveryActionHandler();
                break;
        }

        // enemyAnimation.AnimationHandler(currentEnemyStateAction); // dòng này có cũng được không có cũng được vì case nào sẽ chạy animation đó rồi mà

        // if (lastCheckedCurrentEnemyStateAction != currentEnemyStateAction)
        // {
        //     Debug.Log(currentEnemyStateAction);
        //     lastCheckedCurrentEnemyStateAction = currentEnemyStateAction;
        // }

        // Debug.Log(currentEnemyStateAction);
    }
    
    // ========== STATE ENEMY HANDLERS ==========
    private void PatrolActionHandler()
    {
        Immediately_m_RTCTimer();

        // C1: tối ưu path finding
        if (currentToward != lastCheckedCurrentToward) // dành cho từ trang thái Chase, Jump -> Patrol
        {
            if(currentToward == nullTransform)
            {
                enemyEWPathFindingMovement.MoveTo(lastCheckedCurrentToward.position);
                currentToward = lastCheckedCurrentToward;
            }
            else // currentToward == lastCheckedCurrentToward;  dành cho từ trang thái IDLE -> Patrol
            {
                enemyEWPathFindingMovement.MoveTo(currentToward.position); 
                lastCheckedCurrentToward = currentToward;
            }
        }
        else //currentToward == lastCheckedCurrentToward
        {
            
        }

        // C2: không tối ưu pathfinding
        // enemyPathFindingMovement.MoveTo(currentToward.position); 

        // Debug.Log(Vector3.Distance(EnemyPosition, currentToward.position));
        if (Vector3.Distance(EnemyPosition, currentToward.position) <= PATROL_REACHED_DISTANCE)
        {
            currentEnemyStateAction = EnemyEWStateAction.Idle;
            // m_IdleTimer = idleTimer;
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
            enemyEWPathFindingMovement.StopMovingPhysicalHandler();
        }
        
        if(IsPlayerAround) // alway finds player in here <-- SearchingPlayerAround();
        {
            currentToward = nullTransform; // đổi hướng khi chuyển trạng thái cho đỡ phải tốn chi phí, lại còn tạo ra random hướng của mỗi enemy
            currentEnemyStateAction = EnemyEWStateAction.Chase;
            return;
        }
        // Debug.Log("Is Hole:" + enemySensor.IsHole() + "  ;;;   IfCanJumpOverTheInFrontWall:" + enemySensor.IfCanJumpOverTheInFrontWall());
        if (enemyEWSensor.IsHole() || /*Input.GetKeyDown(KeyCode.L) ||*/ enemyEWSensor.IfCanJumpOverTheInFrontWall())
        {
            // Debug.Log("Is Hole:" + enemySensor.IsHole() + "  ;;;   IfCanJumpOverTheInFrontWall:" + enemySensor.IfCanJumpOverTheInFrontWall());
            currentToward = nullTransform;
            // isJumping = true;
            // enemyPathFindingMovement.hasLeaveGround = true;
            currentEnemyStateAction = EnemyEWStateAction.Jump;
            
            return;
        }
        // ReadyToAttackImmediately();

        // NEW FOR EW @@@
        if(DistanceEnemyToPlayer <= ATTACK_RANGE)
        {
            currentEnemyStateAction = EnemyEWStateAction.Attack;
            return;
        }
    }

    private void IdleActionHandler()
    {
        // if (enemyPathFindingMovement.IsHole() || Input.GetKeyDown(KeyCode.L)) // tmp
        // {
        //     currentEnemyStateAction = EnemyStateAction.Jump;
        //     isJumping = true;
        //     return;
        // }
        enemyEWPathFindingMovement.StopMovingPhysicalHandler();
        Immediately_m_RTCTimer();
        if (m_IdleTimer > 0)
        {
            m_IdleTimer -= Time.deltaTime;
        }
        else
        {
            m_IdleTimer = idleTimer; // reset lại biến thời gian đứng IDLE
            currentEnemyStateAction = EnemyEWStateAction.Patrol;
            // Debug.Log("Đổi thanh Patrol");
            return;
        }
        if(IsPlayerAround)// alway finds player in here <-- SearchingPlayerAround();
        {
            m_IdleTimer = idleTimer;// reset lại biến thời gian đứng IDLE
            currentEnemyStateAction = EnemyEWStateAction.Chase;
            return;
        }
        // ReadyToAttackImmediately();

        // NEW FOR EW @@@
        if(DistanceEnemyToPlayer <= ATTACK_RANGE)
        {
            currentEnemyStateAction = EnemyEWStateAction.Attack;
            return;
        }
    }

    private void ChaseActionHandler()
    {
        enemyEWSensor.AlwayTowardToPlayer();
        Immediately_m_RTCTimer();
        // if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true)
        // {
        //     currentEnemyStateAction = EnemyEWStateAction.ReadyToAttack;
        //     return;
        // }
        // else
        // {
        //     enemyEWPathFindingMovement.MoveTo(PlayerPosition);
        //     // trong lúc đang đuổi theo nên check hố chướng ngại vật các thứ v.v tại đây --> IN HERE <-- tại đây
        //     if(enemyEWPathFindingMovement.IsHavePath() == false) // nếu đang đuổi theo mà không thấy player thì thôi quay về patrol
        //     {
        //         currentEnemyStateAction = EnemyEWStateAction.Patrol;
        //         return;
        //     }
        // }

        enemyEWPathFindingMovement.MoveTo(PlayerPosition);
        // trong lúc đang đuổi theo nên check hố chướng ngại vật các thứ v.v tại đây --> IN HERE <-- tại đây
        if(enemyEWPathFindingMovement.IsHavePath() == false) // nếu đang đuổi theo mà không thấy player thì thôi quay về patrol
        {
            currentEnemyStateAction = EnemyEWStateAction.Patrol;
            return;
        }

        if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
        {
            currentEnemyStateAction = EnemyEWStateAction.Patrol;
            return;
        }
        if (enemyEWSensor.IsHole() || /*Input.GetKeyDown(KeyCode.L) ||*/ enemyEWSensor.IfCanJumpOverTheInFrontWall())
        {
            // Debug.Log("Is Hole:" + enemySensor.IsHole() + "  ;;;   IfCanJumpOverTheInFrontWall:" + enemySensor.IfCanJumpOverTheInFrontWall());
            // isJumping = true;
            // enemyPathFindingMovement.hasLeaveGround = true;
            currentEnemyStateAction = EnemyEWStateAction.Jump;
            
            return;
        }
        
        // NEW FOR EW @@@
        if(DistanceEnemyToPlayer <= ATTACK_RANGE)
        {
            currentEnemyStateAction = EnemyEWStateAction.Attack;
            return;
        }
    }

    private void AttackActionHandler()
    {
        enemyEWPathFindingMovement.StopMovingPhysicalHandler();
        // if (m_RTCTimer > 0) // đợi chờ chờ tới lượt đánh tiếp theo
        // {
        //     currentEnemyStateAction = EnemyEWStateAction.ReadyToAttack;
        //     return;
        // }    
        if (IsPlayerAround == false && DistanceEnemyToPlayer <= ATTACK_DISTANCE)
        {
            // while attacking player and dont see the player
            enemyEWPathFindingMovement.MoveTo(PlayerPosition);
        }
    }

    private void ReadyToAttackActionHandler()
    {
        m_RTCTimer -= Time.deltaTime;
        // viết hàm luôn luôn nhìn về hướng player khi đang ở trạng thái readyTOAttack tại đây
        enemyEWSensor.AlwayTowardToPlayer();
        enemyEWPathFindingMovement.StopMovingPhysicalHandler();
        if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && m_RTCTimer <= 0)
        {
            currentEnemyStateAction = EnemyEWStateAction.Attack;
            return;
        }
        if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
        {
            currentEnemyStateAction = EnemyEWStateAction.Patrol;
            return;
        }
        if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
        IsPlayerAround == true && 
        DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
        {
            currentEnemyStateAction = EnemyEWStateAction.Chase;
            return;
        }
        if(playerMovement.GetPlayerState() == State.Die) // --> error in here  <--
        {
            currentEnemyStateAction = EnemyEWStateAction.Patrol;
            return;
        }
        // Debug.Log("rơi vào nhánh này khi không rơi vào bất kì nhánh nào của RTA");
    }

    private void JumpActionHandler() // xử lý trạng thái STATE khi đang trong state == State.Jump
    {
        // nhảy thì không làm gì khác nữa trừ khi nào mà nhảy xong chạm đất thì thôi mới bắt đầu chuyển trạng thái
        if(enemyEWSensor.IsGrounded() == true && isJumping == false) // đã tiếp đất thì mới được chuyển trạng thái 
        {
            if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false /*&& enemyPathFindingMovement.hasLeaveGround == false*/)
            {
                // Debug.Log("isJumping:" + isJumping);
                // Debug.Log("Jump -> Patrol");
                currentEnemyStateAction = EnemyEWStateAction.Patrol;
                return;
            }
            // if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true /*&& enemyPathFindingMovement.hasLeaveGround == false*/)
            // {
            //     currentEnemyStateAction = EnemyEWStateAction.ReadyToAttack;
            //     return;
            // }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE/* && 
            enemyPathFindingMovement.hasLeaveGround == false*/)
            {
                currentEnemyStateAction = EnemyEWStateAction.Chase;
                return;
            }
            currentEnemyStateAction = EnemyEWStateAction.Chase;
            return;
        }
        else // chưa tiếp đất mà đang trên không // isJumping == true
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
        if (sprites == enemyEWAnimation.AttackSprites && idxFrame == enemyEWAnimation.AttackSprites.Length - 1)
        {
            m_RTCTimer = rTCTimer;
        }
    }
    
    private void TriggerEnemyLastJumpFrameHandler(int idxFrame, Sprite[] sprites)
    {
        if(sprites == enemyEWAnimation.JumpSprites && idxFrame == enemyEWAnimation.JumpSprites.Length)
        {
            // isJumping = false;
        }
    }
    
    private void TriggerEnemyFirstHurtFrameHandler(Sprite[] sprites)
    {
        if(sprites == enemyEWAnimation.HurtSprites)
        {
            
        }
    }
    
    private void TriggerEnemyLastHurtFrameHandler(Sprite[] sprites)
    {
        
        if(sprites == enemyEWAnimation.HurtSprites)
        {
            bool IsPlayerAround = enemyEWSensor.IsSearchedPlayerAround();
            if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true && m_RTCTimer <= 0)
            {
                currentEnemyStateAction = EnemyEWStateAction.Attack;
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                currentEnemyStateAction = EnemyEWStateAction.Chase;
                return;
            }

            // Debug.Log("K nhảy vào state nào cả thì nhảy vào patrol");
            currentEnemyStateAction = EnemyEWStateAction.Patrol; // safe state
            return;
            
        }
    }

    private void TriggerEnemyLastDieFrameHandler(Sprite[] sprites)
    {
        if(sprites == enemyEWAnimation.DeathSprites)
        {
            isDied = true;
        }
    }
    
    private void TriggerEnemyLastRecoveryFrame(Sprite[] sprites)
    {
        if(sprites == enemyEWAnimation.RecoverSprites)
        {
            currentEnemyStateAction = EnemyEWStateAction.Idle;
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
        if(currentEnemyStateAction == EnemyEWStateAction.Hurt)
        {
            return;
        }
        currentEnemyStateAction = EnemyEWStateAction.Hurt;
    }
    
    private void TriggerDieWhenHealthAsZero()
    {
        if(currentEnemyStateAction == EnemyEWStateAction.Die)
        {
            return;
        }
        currentEnemyStateAction = EnemyEWStateAction.Die;

        // set something when enemy is died
        enemyEWPathFindingMovement.StopMovingPhysicalHandler();
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
    
    // private void ReadyToAttackImmediately()
    // {
    //     if(Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) <= READY_TO_ATTACK_DISTANCE && playerMovement.GetPlayerState() != State.Die)
    //     {
    //         if(currentEnemyStateAction == EnemyEWStateAction.Patrol)
    //         {
    //             currentToward = nullTransform;
    //         }
    //         enemyEWSensor.AlwayTowardToPlayer();
    //         currentEnemyStateAction = EnemyEWStateAction.ReadyToAttack;
    //         return;
    //     }
    // }
    
    public void SetIsDied(bool isDied) // hàm này chỉ được tham chiếu bởi EnemySupportTestTool không được tham chiếu hàm này tới bất kì class nào khác
    {
        this.isDied = isDied;
    }
    
    public bool GetIsDied()
    {
        return isDied;
    }
    
    public bool GetIsJumping()
    {
        return isJumping;
    }

    public void SetIsJumpingTrueOutside()
    {
        isJumping = true;
    }

    public void SetIsJumpingFalseOutside()
    {
        isJumping = false;
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
