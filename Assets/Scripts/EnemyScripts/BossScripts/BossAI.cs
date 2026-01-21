using UnityEngine;

public class BossAI : MonoBehaviour
{
    public enum BossStateAction
    {
        Patrol,
        Chase,
        Idle,
        Attack,
        Death,
        Hurt,

        InvisibleSkill1Sprites, // tàng hình, <==> PrepareSkill1
        Visible, // quay trở lại bình thường 

        Skill2, // far attack distance
        PrepareSkill2,

        KeeppInvisible, // đây sẽ là trạng thái quay trờ lại tàng hình ngay khi boss đánh người chơi, còn InvisibleSkill1Sprites chỉ là bắt đầu tàng hình thôi 
        Flee
    }

    public BossStateAction currentEnemyStateAction;
    private BossStateAction lastCheckedCurrentEnemyStateAction;
    private BossPathFindingMovement bossPathFindingMovement = null;
    public Transform currentToward;
    private Transform lastCheckedCurrentToward;
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform chaseLeftPoint;
    public Transform chaseRightPoint;
    private Transform nullTransform = null; // đây sẽ là vị trí mà currentToward hướng tới khi đang trong các trạng thái khác ngoài state Patrol // khi bị Hurt Interupt là không thể vì khi đến gần thì nó tự chuyển thành RTC mà RTC sẽ đổi thanh nullTransform luôn rồi
    private Enemy enemy;
    private BossAnimation bossAnimation;
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
    private BossSensor bossSensor;

    private const float READY_TO_COMBAT_COOLDOWN = 0.5f;
    private const float PATROL_REACHED_DISTANCE = 0.8f; // ngưỡng xác nhận đã tới vị trí tuần tra
    private const float READY_TO_ATTACK_DISTANCE = 1.5f; // ngưỡng enemy sẽ vào trạng thái chuẩn bị tấn công
    private const float ATTACK_DISTANCE = 1f; // ngưỡng mà enemy sẽ tấn công
    private const float  DISENGAGE_DISTANCE = 4f; // ngưỡng mà enemy quyết định còn đuổi hay không đuổi tiếp ??? nó như là MAX_CHASE vậy
    private const float CHASE_MIN_DISTANCE = 2f; // ngưỡng mà enemy quyết định còn đuổi hay không đuổi tiếp ??? nó như là MIN_CHASE vậy

    private BossSkill1 bossSkill1;
    private HealthHandler bossHealthHandler;

    private void Awake()
    {
        bossPathFindingMovement = gameObject.GetComponent<BossPathFindingMovement>();
        enemy = gameObject.GetComponent<Enemy>();
        bossAnimation = gameObject.GetComponent<BossAnimation>();
        enemyHealthHandler = gameObject.GetComponent<HealthHandler>();

        bossSensor = gameObject.GetComponent<BossSensor>();

        bossSkill1 = gameObject.GetComponent<BossSkill1>();

        bossHealthHandler = gameObject.GetComponent<HealthHandler>();
    }

    private void Start()
    {
        currentEnemyStateAction = BossStateAction.Patrol;
        currentToward = UnityEngine.Random.Range(-1, 1) > 0 ? rightPoint : leftPoint;
        lastCheckedCurrentToward = currentToward == rightPoint ? leftPoint : rightPoint;

        enemyHealthSystem = enemyHealthHandler.GetHealthSystem();
        enemyHealthBar = enemyHealthHandler.GetHealthBar();

        bossAnimation.OnTriggerEachFrames += TriggerEnemyLastAttackFrameHandler;
        bossAnimation.OnTriggerLastFrames += TriggerEnemyLastHurtFrameHandler;
        bossAnimation.OnTriggerLastFrames += TriggerEnemyLastDieFrameHandler;
        bossAnimation.OnTriggerLastFrames += TriggerBossLastPrepareSkill2FrameHandler;
        bossAnimation.OnTriggerLastFrames += TriggerBossLastInvisibleSkill1FrameHandler;

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

        // test
        if (Input.GetKeyDown(KeyCode.G))
        {
            currentEnemyStateAction = BossStateAction.InvisibleSkill1Sprites;
        }
        

        IsPlayerAround = bossSensor.IsSearchedPlayerAround();
        PlayerPosition = Player.Instance.GetPlayerPosition();
        EnemyPosition = BossPositionHolder.Instance.GetRealBossPosition();
        DistanceEnemyToPlayer = Vector3.Distance(PlayerPosition, EnemyPosition);

        // Patrol, Chase, Idle, Attack, Death, Hurt, InvisibleSkill1Sprites, Visible, Skill2, PrepareSkill2

        switch (currentEnemyStateAction)
        {
            case BossStateAction.Patrol:
                bossAnimation.AnimationHandler(BossState.Walk);
                PatrolActionHandler();
                break;
            case BossStateAction.Chase:
                bossAnimation.AnimationHandler(BossState.Walk);
                ChaseActionHandler();
                break;
            case BossStateAction.Idle:
                bossAnimation.AnimationHandler(BossState.Idle);
                IdleActionHandler();
                break;
            case BossStateAction.Attack:
                bossAnimation.AnimationHandler(BossState.Attack);
                AttackActionHandler();
                break;
            case BossStateAction.Hurt:
                bossAnimation.AnimationHandler(BossState.Hurt);
                HurtActionHandler();
                break;
            case BossStateAction.Death:
                bossAnimation.AnimationHandler(BossState.Death);
                DeathActionHandler();
                break;
            case BossStateAction.PrepareSkill2: // cái này sẽ gọi skill 2
                bossAnimation.AnimationHandler(BossState.PrepareSkill2);
                PrepareSkill2ActionHandler();
                break;
            case BossStateAction.InvisibleSkill1Sprites:
                bossAnimation.AnimationHandler(BossState.InvisibleSkill1Sprites);
                InvisibleSkill1Handler();
                break;
            case BossStateAction.KeeppInvisible:
                bossAnimation.AnimationHandler(BossState.KeeppInvisible);
                KeepInVisibleHandler();
                break;
            case BossStateAction.Flee:
                bossAnimation.AnimationHandler(BossState.KeeppInvisible);
                FleeHandler();
                break;
            
        }

        if (lastCheckedCurrentEnemyStateAction != currentEnemyStateAction)
        {
            Debug.Log(currentEnemyStateAction);
            lastCheckedCurrentEnemyStateAction = currentEnemyStateAction;
        }

        // Debug.Log(currentEnemyStateAction);
    }
    

    // ========== STATE ENEMY HANDLERS ==========
    private void PatrolActionHandler()
    {
        // Immediately_m_RTCTimer();

        // C1: tối ưu path finding
        if (currentToward != lastCheckedCurrentToward) // dành cho từ trang thái Chase, Jump -> Patrol
        {
            if(currentToward == nullTransform)
            {
                bossPathFindingMovement.MoveTo(lastCheckedCurrentToward.position);
                currentToward = lastCheckedCurrentToward;
            }
            else // currentToward == lastCheckedCurrentToward;  dành cho từ trang thái IDLE -> Patrol
            {
                bossPathFindingMovement.MoveTo(currentToward.position); 
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
            currentEnemyStateAction = BossStateAction.Idle;
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
            bossPathFindingMovement.StopMovingPhysicalHandler();
        }
        
        if(IsPlayerAround) // alway finds player in here <-- SearchingPlayerAround();
        {
            currentToward = nullTransform; // đổi hướng khi chuyển trạng thái cho đỡ phải tốn chi phí, lại còn tạo ra random hướng của mỗi enemy
            currentEnemyStateAction = BossStateAction.Chase;
            return;
        }
    }

    private void IdleActionHandler()
    {
        bossPathFindingMovement.StopMovingPhysicalHandler();

        if (m_IdleTimer > 0)
        {
            m_IdleTimer -= Time.deltaTime;
        }
        else
        {
            m_IdleTimer = idleTimer; // reset lại biến thời gian đứng IDLE
            currentEnemyStateAction = BossStateAction.Patrol;
            // Debug.Log("Đổi thanh Patrol");
            return;
        }
        if(IsPlayerAround)// alway finds player in here <-- SearchingPlayerAround();
        {
            m_IdleTimer = idleTimer;// reset lại biến thời gian đứng IDLE
            currentEnemyStateAction = BossStateAction.Chase;
            return;
        }
    }

    private void ChaseActionHandler()
    {
        bossSensor.AlwayTowardToPlayer();

        // NEW FOR BOSS @@@
        bossPathFindingMovement.MoveTo(PlayerPosition);
        // trong lúc đang đuổi theo nên check hố chướng ngại vật các thứ v.v tại đây --> IN HERE <-- tại đây
        if(bossPathFindingMovement.IsHavePath() == false) // nếu đang đuổi theo mà không thấy player thì thôi quay về patrol
        {
            currentEnemyStateAction = BossStateAction.Patrol;
            return;
        }
        

        if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
        {
            currentEnemyStateAction = BossStateAction.Patrol;
            return;
        }
    }

    private void AttackActionHandler()
    {
        bossPathFindingMovement.StopMovingPhysicalHandler();
        // if (m_RTCTimer > 0) // đợi chờ chờ tới lượt đánh tiếp theo
        // {
        //     currentEnemyStateAction = EnemyStateAction.ReadyToAttack;
        //     return;
        // }    
        if (IsPlayerAround == false && DistanceEnemyToPlayer <= ATTACK_DISTANCE)
        {
            // while attacking player and dont see the player
            bossPathFindingMovement.MoveTo(PlayerPosition);
        }
    }

    private void ReadyToAttackActionHandler()
    {
        m_RTCTimer -= Time.deltaTime;
        // viết hàm luôn luôn nhìn về hướng player khi đang ở trạng thái readyTOAttack tại đây
        bossSensor.AlwayTowardToPlayer();
        bossPathFindingMovement.StopMovingPhysicalHandler();
        if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && m_RTCTimer <= 0)
        {
            currentEnemyStateAction = BossStateAction.Attack;
            return;
        }
        if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
        {
            currentEnemyStateAction = BossStateAction.Patrol;
            return;
        }
        if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
        IsPlayerAround == true && 
        DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
        {
            currentEnemyStateAction = BossStateAction.Chase;
            return;
        }
        if(playerMovement.GetPlayerState() == State.Die) // --> error in here  <--
        {
            currentEnemyStateAction = BossStateAction.Patrol;
            return;
        }
        // Debug.Log("rơi vào nhánh này khi không rơi vào bất kì nhánh nào của RTA");
    }

    private void JumpActionHandler() // xử lý trạng thái STATE khi đang trong state == State.Jump
    {
        // nhảy thì không làm gì khác nữa trừ khi nào mà nhảy xong chạm đất thì thôi mới bắt đầu chuyển trạng thái
        if(bossSensor.IsGrounded() == true && isJumping == false) // đã tiếp đất thì mới được chuyển trạng thái 
        {
            if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false /*&& enemyPathFindingMovement.hasLeaveGround == false*/)
            {
                // Debug.Log("isJumping:" + isJumping);
                // Debug.Log("Jump -> Patrol");
                currentEnemyStateAction = BossStateAction.Patrol;
                return;
            }
            // if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true /*&& enemyPathFindingMovement.hasLeaveGround == false*/)
            // {
            //     currentEnemyStateAction = EnemyStateAction.ReadyToAttack;
            //     return;
            // }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE/* && 
            enemyPathFindingMovement.hasLeaveGround == false*/)
            {
                currentEnemyStateAction = BossStateAction.Chase;
                return;
            }
            currentEnemyStateAction = BossStateAction.Chase;
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
    
    private void PrepareSkill2ActionHandler()
    {
        // sẽ không làm gì khi đang thực hiện prepare skill 2, chỉ thực hiện ở frame cuối của nó
    }
    
    private void InvisibleSkill1Handler()
    {
        // Debug.Log("đang ở InvisibleSkill1Handler");

    }
    
    private void KeepInVisibleHandler()
    {
        if(bossHealthHandler.GetHP() >= 30)
        {
            // attack, chase ???
            if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && m_RTCTimer <= 0)
            {
                currentEnemyStateAction = BossStateAction.Attack;
                return;
            }
            if(IsPlayerAround)// alway finds player in here <-- SearchingPlayerAround();
            {
                m_IdleTimer = idleTimer;// reset lại biến thời gian đứng IDLE
                currentEnemyStateAction = BossStateAction.Chase;
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                currentEnemyStateAction = BossStateAction.Chase;
                return;
            }
        }
        else if(bossHealthHandler.GetHP() < 30)
        {
            // flee
            currentEnemyStateAction = BossStateAction.Flee;
            return;
        }
    }
    
    private void FleeHandler()
    {
        // continue your work in here --> IN HERE <--
        Vector3 fleeDir = -(PlayerPosition - EnemyPosition).normalized;
        float fleeDistance = 5f;
        Vector3 fleeTarget = EnemyPosition + fleeDir * fleeDistance;
        bossPathFindingMovement.MoveTo(fleeTarget);
    }
    // =============================================================



    // =============== HANDLER ENEMY ANIMATION BY EACH FRAMES ===============
    private void TriggerEnemyLastAttackFrameHandler(int idxFrame, Sprite[] sprites)
    {
        if (sprites == bossAnimation.AttackSprites && idxFrame == bossAnimation.AttackSprites.Length - 1)
        {
            m_RTCTimer = rTCTimer;
        }
    }
    
    private void TriggerEnemyLastHurtFrameHandler(Sprite[] sprites)
    {
        
        if(sprites == bossAnimation.HurtSprites)
        {
            bool IsPlayerAround = bossSensor.IsSearchedPlayerAround();
            if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true && m_RTCTimer <= 0)
            {
                currentEnemyStateAction = BossStateAction.Attack;
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                currentEnemyStateAction = BossStateAction.Chase;
                return;
            }

            // Debug.Log("K nhảy vào state nào cả thì nhảy vào patrol");
            currentEnemyStateAction = BossStateAction.Patrol; // safe state
            return;
            
        }
    }

    private void TriggerEnemyLastDieFrameHandler(Sprite[] sprites)
    {
        if(sprites == bossAnimation.DeathSprites)
        {
            isDied = true;
        }
    }
    
    private void TriggerBossLastPrepareSkill2FrameHandler(Sprite[] sprites)
    {
        if(sprites == bossAnimation.PrepareSkill2Sprites)
        {
            // gọi skill 2 vã vào đầu thằng player: đã được gọi rồi bên BossCallerSkill2 nhưng chuyển trạng thái sáng trạng thái khác tại đây
            bool IsPlayerAround = bossSensor.IsSearchedPlayerAround();
            if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true && m_RTCTimer <= 0)
            {
                currentEnemyStateAction = BossStateAction.Attack;
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                currentEnemyStateAction = BossStateAction.Chase;
                return;
            }

            // Debug.Log("K nhảy vào state nào cả thì nhảy vào patrol");
            currentEnemyStateAction = BossStateAction.Patrol; // safe state
            return;
        }
        
    }
    
    private void TriggerBossLastInvisibleSkill1FrameHandler(Sprite[] sprites)
    {
        if(sprites == bossAnimation.InvisibleSkill1Sprites)
        {
            bossSkill1.SetDefaultValueForSkill1(); // bắt đầu tàng hình
            currentEnemyStateAction = BossStateAction.KeeppInvisible;

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
        if(currentEnemyStateAction == BossStateAction.Hurt)
        {
            return;
        }
        currentEnemyStateAction = BossStateAction.Hurt;
    }
    
    private void TriggerDieWhenHealthAsZero()
    {
        if(currentEnemyStateAction == BossStateAction.Death)
        {
            return;
        }
        currentEnemyStateAction = BossStateAction.Death;

        // set something when enemy is died
        bossPathFindingMovement.StopMovingPhysicalHandler();
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        enemyHealthBar.gameObject.SetActive(false);
    }
    
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
