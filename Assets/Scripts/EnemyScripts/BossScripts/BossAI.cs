using Unity.VisualScripting;
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

        InvisibleSkill1, // tàng hình, <==> PrepareSkill1
        Visible, // quay trở lại bình thường 

        Skill2, // far attack distance
        PrepareSkill2,

        KeeppInvisible, // đây sẽ là trạng thái quay trờ lại tàng hình ngay khi boss đánh người chơi, còn InvisibleSkill1Sprites chỉ là bắt đầu tàng hình thôi 
        Flee,
        Recover // đợi chờ sau mỗi lượt đánh đây cũng chính là RTC vì boss animation không có RTC => phải dùng tạm Idle
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
    private float attackCoolDown = 1f; // thời gian chờ hồi lượt đánh tiếp theo
    private float timer_AttackCoolDown; // thời gian chờ idle để chuyển thành attack // nếu biến này bị Hurt interupt mà không reset có bị sao không nhỉ ?  (lười mò để check quá :D, check sau tại đây nếu có lỗi liên quan tới Idle và RTC)

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
    private const float ATTACK_DISTANCE = 1f; // ngưỡng mà enemy sẽ tấn công: old = 1f
    private const float  DISENGAGE_DISTANCE = 4f; // ngưỡng mà enemy quyết định còn đuổi hay không đuổi tiếp ??? nó như là MAX_CHASE vậy
    private const float CHASE_MIN_DISTANCE = 2f; // ngưỡng mà enemy quyết định còn đuổi hay không đuổi tiếp ??? nó như là MIN_CHASE vậy
    private const float MIN_DISTANCE_TO_PLAYER = 0.8f;

    private BossSkill1 bossSkill1;
    private HealthHandler bossHealthHandler;
    private BossCallerSkill2 bossCallerSkill2;

    private bool IsHavePath;

    private void Awake()
    {
        bossPathFindingMovement = gameObject.GetComponent<BossPathFindingMovement>();
        enemy = gameObject.GetComponent<Enemy>();
        bossAnimation = gameObject.GetComponent<BossAnimation>();
        enemyHealthHandler = gameObject.GetComponent<HealthHandler>();

        bossSensor = gameObject.GetComponent<BossSensor>();

        bossSkill1 = gameObject.GetComponent<BossSkill1>();

        bossHealthHandler = gameObject.GetComponent<HealthHandler>();

        bossCallerSkill2 = gameObject.GetComponent<BossCallerSkill2>();
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
        // bossAnimation.OnTriggerLastFrames += TriggerEnemyLastDieFrameHandler;
        bossAnimation.OnTriggerLastFrames += TriggerBossLastPrepareSkill2FrameHandler;
        bossAnimation.OnTriggerLastFrames += TriggerBossLastInvisibleSkill1FrameHandler;
        bossAnimation.OnTriggerLastFrames += TriggerBossLastVisibleFrameHandler;
        bossAnimation.OnTriggerLastFrames += TriggerBossLastDieFrameHandle;
        //  bossAnimation.OnTriggerEachFrames += TriggerBossFirstInvisibleSkill1FrameHandler;

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

        // testetn
        if (Input.GetKeyDown(KeyCode.G)/* && bossSkill1.getCanUseSkill1() == true*/)
        {
            currentEnemyStateAction = BossStateAction.InvisibleSkill1;
            bossSkill1.UseSkill1();
            bossSkill1.SetSkill1CoolDown(); // cái này sẽ đặt lại khi mà hết tàng hình
        }
        else if(Input.GetKeyDown(KeyCode.F) && bossCallerSkill2.getCanUseSkill2() == true)
        {
            // currentEnemyStateAction = BossStateAction.PrepareSkill2;
            // bossCallerSkill2.SetSkill2CoolDown(); // cái này sẽ đặt lại khi mà skill 2 được hoàn tất triển khai
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            bossHealthHandler.Heal(100);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            bossHealthHandler.Damage(80);
        }
        

        IsPlayerAround = bossSensor.IsSearchedPlayerAround();
        PlayerPosition = Player.Instance.GetPlayerPosition();
        EnemyPosition = BossPositionHolder.Instance.GetRealBossPosition();
        DistanceEnemyToPlayer = Vector2.Distance(PlayerPosition, EnemyPosition);
        IsHavePath = bossPathFindingMovement.IsHavePath();

        // test var
        if(Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("DistanceEnemyToPlayer: " + DistanceEnemyToPlayer);
        }

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
            case BossStateAction.InvisibleSkill1:
                bossAnimation.AnimationHandler(BossState.InvisibleSkill1);
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
            case BossStateAction.Visible:
                bossAnimation.AnimationHandler(BossState.Visible);
                VisibleHandler();
                break;
            case BossStateAction.Recover:
                bossAnimation.AnimationHandler(BossState.Recover);
                RecoverHandler();
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
        if (Vector2.Distance(EnemyPosition, currentToward.position) <= PATROL_REACHED_DISTANCE)
        {
            // Debug.Log("Patrol -> Idle");
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
        
        if(IsPlayerAround || DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && playerMovement.GetPlayerState() != State.Die) // alway finds player in here <-- SearchingPlayerAround();
        {
            bossSensor.AlwayTowardToPlayer();
            // Debug.Log("Patrol -> Chase");
            currentToward = nullTransform; // đổi hướng khi chuyển trạng thái cho đỡ phải tốn chi phí, lại còn tạo ra random hướng của mỗi enemy
            currentEnemyStateAction = BossStateAction.Chase;
            return;
        }
    }

    private void IdleActionHandler()
    {
        if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0)
        {
            currentEnemyStateAction = BossStateAction.Attack;
            return;
        }

        bossPathFindingMovement.StopMovingPhysicalHandler();

        if (m_IdleTimer > 0)
        {
            m_IdleTimer -= Time.deltaTime;
        }
        else
        {
            // Debug.Log("Idle -> Patrol");
            m_IdleTimer = idleTimer; // reset lại biến thời gian đứng IDLE
            currentEnemyStateAction = BossStateAction.Patrol;
            // Debug.Log("Đổi thanh Patrol");
            return;
        }
        if(IsPlayerAround)// alway finds player in here <-- SearchingPlayerAround();
        {
            // Debug.Log("Idle -> Chase");
            m_IdleTimer = idleTimer;// reset lại biến thời gian đứng IDLE
            currentEnemyStateAction = BossStateAction.Chase;
            return;
        }

        if (DistanceEnemyToPlayer <= DISENGAGE_DISTANCE)
        {
            bossSensor.AlwayTowardToPlayer();
        }
            
    }

    private void ChaseActionHandler()
    {
        bossSensor.AlwayTowardToPlayer();
        DeclineTimerAttackCoolDown();
        SetUpBossWhenPlayerDied();

        // NEW FOR BOSS @@@
        bossPathFindingMovement.MoveTo(PlayerPosition);
        // trong lúc đang đuổi theo nên check hố chướng ngại vật các thứ v.v tại đây --> IN HERE <-- tại đây
        if(bossPathFindingMovement.IsHavePath() == false) // nếu đang đuổi theo mà không thấy player thì thôi quay về patrol
        {
            Debug.Log("Chase -> Patrol 1");
            currentEnemyStateAction = BossStateAction.Patrol;
            return;
        }
        

        if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
        {
            Debug.Log("Chase -> Patrol 2");
            currentEnemyStateAction = BossStateAction.Patrol;
            return;
        }
        
        // use skill
        if(bossCallerSkill2.getCanUseSkill2() == true && DistanceEnemyToPlayer >= READY_TO_ATTACK_DISTANCE)
        {
            Debug.Log("Chase -> PrepareSkill2");
            currentEnemyStateAction = BossStateAction.PrepareSkill2;
            bossCallerSkill2.SetSkill2CoolDown(); // cái này sẽ đặt lại khi mà skill 2 được hoàn tất triển khai
            return;
        }
        if(bossSkill1.getCanUseSkill1() == true && DistanceEnemyToPlayer >= READY_TO_ATTACK_DISTANCE)
        {
            Debug.Log("Chase -> InvisibleSkill1");
            currentEnemyStateAction = BossStateAction.InvisibleSkill1;
            bossSkill1.UseSkill1();
            return;
        }

        if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0)
        {
            Debug.Log("Chase -> Recover");
            currentEnemyStateAction = BossStateAction.Recover;
            return;
        }
        if(DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true/* && timer_AttackCoolDown > 0*/)// timer_AttackCoolDown > 0 ???
        {
            bossPathFindingMovement.StopMovingPhysicalHandler(); 
        }
        Debug.Log("Chase -> Null");
    }

    private void AttackActionHandler()
    {
        bossPathFindingMovement.StopMovingPhysicalHandler();
        // SetUpBossWhenPlayerDied();
        if (timer_AttackCoolDown > 0) // đợi chờ chờ tới lượt đánh tiếp theo
        {
            // Debug.Log("Attack -> Recover");
            currentEnemyStateAction = BossStateAction.Recover;
            return;
        }    

        if (IsPlayerAround == false && DistanceEnemyToPlayer <= DISENGAGE_DISTANCE)
        {
            // while attacking player and dont see the player
            bossPathFindingMovement.MoveTo(PlayerPosition);
        }
    }

    private void ReadyToAttackActionHandler()
    {
        DeclineTimerAttackCoolDown();
        // viết hàm luôn luôn nhìn về hướng player khi đang ở trạng thái readyTOAttack tại đây
        bossSensor.AlwayTowardToPlayer();
        bossPathFindingMovement.StopMovingPhysicalHandler();
        if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0)
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
        bossSkill1.SetDefaultValueForSkill1(); // cho đầu frame ??? Invisible ??
    }
    
    private void KeepInVisibleHandler()
    {
        DeclineTimerAttackCoolDown();
        if(bossHealthHandler.GetHP() >= 30)
        {
            if (DistanceEnemyToPlayer <= DISENGAGE_DISTANCE)
            {
                bossSensor.AlwayTowardToPlayer();
            }
            // attack, chase ???
            if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0)
            {
                // Debug.Log("KeepInvisible -> Attack");
                currentEnemyStateAction = BossStateAction.Attack;
                return;
            }
            if(IsPlayerAround/* && DistanceEnemyToPlayer <= CHASE_MIN_DISTANCE*/)// alway finds player in here <-- SearchingPlayerAround();
            {
                // Debug.Log("KeepInvisible -> MoveTo Player");
                if(DistanceEnemyToPlayer <= 1f) // nếu nhìn thấy người chơi mà gần quá thì thôi dừng không lại đẩy người chơi
                {
                    bossPathFindingMovement.StopMovingPhysicalHandler();
                    return;
                }
                bossPathFindingMovement.MoveTo(PlayerPosition); // di chuyển tới người chơi trong trạng thái tàng hình
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                // Debug.Log("KeepInvisible -> Chase");
                currentEnemyStateAction = BossStateAction.Chase;
                return;
            }
            // Debug.Log("KeepInVisible -> null"); bossPathFindingMovement.StopMovingPhysicalHandler();
        }
        else if(bossHealthHandler.GetHP() < 30)
        {
            bossPathFindingMovement.StopMovingPhysicalHandler(); // dừng lại luôn không là nó chạy tới một đoạn tới người chơi rồi mới flee đi hướng khác
            // flee
            // Debug.Log("KeepInvisible -> Flee");
            currentEnemyStateAction = BossStateAction.Flee;
            return;
        }
        if(bossSkill1.getCanKeepUseSkill1() == false || playerMovement.GetPlayerState() == State.Die) // không thể sử dụng skill 1 hoặc player chết thì hủy tàng hình
        {
            // Debug.Log("KeepInvisible -> Visible");
            currentEnemyStateAction = BossStateAction.Visible;
            return;
        }
    }
    
    private void FleeHandler()
    {
        // Debug.Log("DistanceEnemyToPlayer:" + DistanceEnemyToPlayer);
        if(/*IsPlayerAround == false ||*/ DistanceEnemyToPlayer >= DISENGAGE_DISTANCE) // nếu người ở xa lúc đang flee
        {
            // Debug.Log("Flee Attack");
            // đứng yên hồi máu thôi khi nào đầy rồi thì tấn công
            // bossPathFindingMovement.StopMovingPhysicalHandler();

            // bossPathFindingMovement.MoveTo(PlayerPosition);
            bossPathFindingMovement.StopMovingPhysicalHandler();

            // if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0)
            // {
            //     // Debug.Log("Flee -> Attack");
            //     currentEnemyStateAction = BossStateAction.Attack;
            //     return;
            // }
            // Debug.Log("Flee -> Flee Attack -> Null"); bossPathFindingMovement.StopMovingPhysicalHandler();
        }
        else if(/*IsPlayerAround == true || */DistanceEnemyToPlayer < DISENGAGE_DISTANCE) // nếu player gần lúc đang flee
        {
            // Debug.Log("Flee Defense");
            if (!bossPathFindingMovement.IsHavePath()) // phải chạy tới fleeTarget xong rồi mới gọi tiếp cái tiếp theo
            {
                // nếu fleeTarget mà ngoài thì đổi hướng
                // if(DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE) // nếu người chơi đến gần rồi thì mới bỏ chạy
                // {
                //     // Debug.Log("Flee -> Flee Defense -> Null Path -> Move To Player");
                //     bossPathFindingMovement.MoveTo(bossPathFindingMovement.FindValidFleeTarget(EnemyPosition, PlayerPosition));
                // }
                // else
                // {
                //     // Debug.Log("Flee -> Flee Defense ->  Null Path -> Stop");
                //     bossPathFindingMovement.StopMovingPhysicalHandler(); // còn không thì đứng yên
                // }
                // KHI FLEE THÌ CHỈ KHI NÀO ĐỨNG IM THÌ MỚI HỒI ĐƯỢC HP CÒN DÙ ĐANG FLEE NHƯNG KHÔNG ĐỨNG IM THÌ KHÔNG HỒI ĐƯỢC HP
                bossPathFindingMovement.MoveTo(bossPathFindingMovement.FindValidFleeTarget(EnemyPosition, PlayerPosition)); // nếu người chơi ở gần và chưa có đường thì tìm đường flee
            }
            else // bossPathFindingMovement.IsHavePath() == true
            {
                // nếu đang chạy mà gặp tường thì đổi hướng ? vì mfn của boss chỉ thiết kế trên mặt phẳng 
                if(bossSensor.IsWallOrGroundInFrontForBossCheck() == true) // có đường Flee nhưng gặp tường => tele và chuyển hướng bằng cách tính một Flee khác
                {
                    // Debug.Log("Flee -> Flee Defense ->  Have Path -> Stop");
                    // viết một hàm dịch chuyển sau người chơi và chọn vị trí hợp lệ
                    bossPathFindingMovement.Teleport(PlayerPosition, EnemyPosition, playerMovement.GetPlayerVisualDirection());
                    bossPathFindingMovement.MoveTo(bossPathFindingMovement.FindValidFleeTarget(EnemyPosition, PlayerPosition));
                    bossPathFindingMovement.StopMovingPhysicalHandler();
                }
                // Debug.Log("Flee -> Flee Defense ->  Have Path -> Null");
            }
            // Debug.Log("Flee -> Flee Defense -> Null");
        }
        

        // nếu hp cực thấp thì được quyển teleport

        if(bossHealthHandler.GetHP() >= 30) // quay trở lại state
        {
            // Debug.Log("Flee -> KeeppInvisible");
            currentEnemyStateAction = BossStateAction.KeeppInvisible;
            return;
        }
        if(bossSkill1.getCanKeepUseSkill1() == false)
        {
            // Debug.Log("Flee -> Visible");
            currentEnemyStateAction = BossStateAction.Visible;
            return;
        }
    }
    
    private void VisibleHandler()
    {
        
    }
    
    private void RecoverHandler()
    {
        DeclineTimerAttackCoolDown();
        // viết hàm luôn luôn nhìn về hướng player khi đang ở trạng thái readyTOAttack tại đây
        bossSensor.AlwayTowardToPlayer();
        bossPathFindingMovement.StopMovingPhysicalHandler();
        if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0)
        {
            Debug.Log("Recover -> Attack");
            currentEnemyStateAction = BossStateAction.Attack;
            return;
        }
        if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
        {
            Debug.Log("Recover -> Patrol 1");
            Immediately_timer_AttackCoolDownAsZero();
            currentEnemyStateAction = BossStateAction.Patrol;
            return;
        }
        if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
        IsPlayerAround == true && 
        DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
        {
            Debug.Log("Recover -> Chase");
            Immediately_timer_AttackCoolDownAsZero();
            currentEnemyStateAction = BossStateAction.Chase;
            return;
        }
        if(playerMovement.GetPlayerState() == State.Die) // --> error in here  <--
        {
            Debug.Log("Recover -> Patrol 2");
            Immediately_timer_AttackCoolDownAsZero();
            currentEnemyStateAction = BossStateAction.Patrol;
            return;
        }

        if(DistanceEnemyToPlayer > ATTACK_DISTANCE && IsPlayerAround == true) // nếu xa thì đi lại gần để không bị đứng im khi tấn công, nhưng phải nhìn thấy người chơi
        {
            Debug.Log("Recover -> Chase");
            // bossPathFindingMovement.MoveTo(PlayerPosition);
            currentEnemyStateAction = BossStateAction.Chase;
            return;
        }
        else if(DistanceEnemyToPlayer <= ATTACK_DISTANCE) // nếu gần rồi thì dừng không lại đẩy người chơi
        {
            Debug.Log("Recover -> Stop");
            bossPathFindingMovement.StopMovingPhysicalHandler();
            return;
        }
        
        // if(DistanceEnemyToPlayer >= ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0) // đi gần tới để sẵn sàng tấn công player? có thể sẽ sai ??? testing
        // {
        //     bossPathFindingMovement.MoveTo(PlayerPosition);
        // }
        
        // safe state
        // Debug.Log("Recover -> Null -> Patrol");
        // Immediately_timer_AttackCoolDownAsZero();
        // currentEnemyStateAction = BossStateAction.Patrol;
        // return;

    }
    // =============================================================



    // =============== HANDLER ENEMY ANIMATION BY EACH FRAMES ===============
    private void TriggerEnemyLastAttackFrameHandler(int idxFrame, Sprite[] sprites)
    {
        if (sprites == bossAnimation.AttackSprites && idxFrame == bossAnimation.AttackSprites.Length - 1)
        {
            timer_AttackCoolDown = attackCoolDown;
            SetUpBossWhenPlayerDied();
            if(bossSkill1.getCanKeepUseSkill1() == true)
            {
                // Debug.Log("LastAttack -> KeeppInvisible");
                currentEnemyStateAction = BossStateAction.KeeppInvisible;
                return;
            }

            // if (DistanceEnemyToPlayer <= ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown > 0) // đợi chờ chờ tới lượt đánh tiếp theo
            // {
            //     Debug.Log("LastAttack -> Idle");
            //     currentEnemyStateAction = BossStateAction.Idle;
            //     return;
            // } 

            // if(bossSkill1.getCanKeepUseSkill1() == true)
            // {
            //     Debug.Log("LastAttack -> KeeppInvisible");
            //     currentEnemyStateAction = BossStateAction.KeeppInvisible;
            //     return;
            // }
            // if(IsPlayerAround || DistanceEnemyToPlayer < DISENGAGE_DISTANCE) // nếu player vẫn được nhìn thấy hoặc vẫn trong khoảng cách chưa từ bỏ
            // {
            //     Debug.Log("LastAttack -> Chase");  
            //     currentEnemyStateAction = BossStateAction.Chase;
            //     return;
            // }
            // if (DistanceEnemyToPlayer >= DISENGAGE_DISTANCE && IsPlayerAround == false)
            // {
            //     Debug.Log("LastAttack -> Patrol");
            //     currentEnemyStateAction = BossStateAction.Patrol;
            //     return;
            // }
            // Debug.Log("LastAttack -> Patrol");
            // // Debug.Log("K nhảy vào state nào cả thì nhảy vào patrol");
            // currentEnemyStateAction = BossStateAction.Patrol; // safe state
            // return;
        }
    }
    
    private void TriggerEnemyLastHurtFrameHandler(Sprite[] sprites)
    {
        
        if(sprites == bossAnimation.HurtSprites)
        {
            // SetUpBossWhenPlayerDied();
            bool IsPlayerAround = bossSensor.IsSearchedPlayerAround();

            if(bossSkill1.getCanKeepUseSkill1() == true)
            {
                // Debug.Log("LastHurt -> KeeppInvisible");
                currentEnemyStateAction = BossStateAction.KeeppInvisible;
                return;
            }
            if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0)
            {
                // Debug.Log("LastHurt -> Attack");
                currentEnemyStateAction = BossStateAction.Attack;
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                // Debug.Log("LastHurt -> Chase");
                currentEnemyStateAction = BossStateAction.Chase;
                return;
            }
            // Debug.Log("LastHurt -> Patrol");
            // Debug.Log("K nhảy vào state nào cả thì nhảy vào patrol");
            currentEnemyStateAction = BossStateAction.Patrol; // safe state
            return;
            
        }
    }

    // private void TriggerEnemyLastDieFrameHandler(Sprite[] sprites)
    // {
    //     if(sprites == bossAnimation.DeathSprites)
    //     {
    //         isDied = true;
    //     }
    // }
    
    private void TriggerBossLastPrepareSkill2FrameHandler(Sprite[] sprites)
    {
        if(sprites == bossAnimation.PrepareSkill2Sprites)
        {
            // SetUpBossWhenPlayerDied();
            // gọi skill 2 vã vào đầu thằng player: đã được gọi rồi bên BossCallerSkill2 nhưng chuyển trạng thái sáng trạng thái khác tại đây
            bool IsPlayerAround = bossSensor.IsSearchedPlayerAround();
            if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0)
            {
                // Debug.Log("PrepareSkill2 -> Attack");
                currentEnemyStateAction = BossStateAction.Attack;
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                // Debug.Log("PrepareSkill2 -> Chase");
                currentEnemyStateAction = BossStateAction.Chase;
                return;
            }

            // Debug.Log("K nhảy vào state nào cả thì nhảy vào patrol");
            // Debug.Log("PrepareSkill2 -> Patrol");
            currentEnemyStateAction = BossStateAction.Patrol; // safe state
            return;
        }
        
    }
    
    // private void TriggerBossFirstInvisibleSkill1FrameHandler(int idxFrame, Sprite[] sprites)
    // {
    //     if(idxFrame == 0 && sprites == bossAnimation.InvisibleSkill1Sprites)
    //     {
    //         bossSkill1.SetDefaultValueForSkill1();
    //     }
    // }
    
    private void TriggerBossLastInvisibleSkill1FrameHandler(Sprite[] sprites)
    {
        if(sprites == bossAnimation.InvisibleSkill1Sprites)
        {
            // bossSkill1.SetDefaultValueForSkill1(); // bắt đầu tàng hình
            // Debug.Log("LastInvisibleSkill1 -> KeeppInvisible");
            currentEnemyStateAction = BossStateAction.KeeppInvisible;

        }
    }
    
    private void TriggerBossLastVisibleFrameHandler(Sprite[] sprites)
    {
        if(sprites == bossAnimation.VisibleSprites)
        {
            // SetUpBossWhenPlayerDied();
            // reset timer cooldown skill1
            // Debug.Log("Reset coolDown Skill 1");
            bossSkill1.SetSkill1CoolDown(); // cái này sẽ đặt lại khi mà skill 2 được hoàn tất triển khai

    
            bool IsPlayerAround = bossSensor.IsSearchedPlayerAround();
            if (DistanceEnemyToPlayer <= READY_TO_ATTACK_DISTANCE && IsPlayerAround == true && timer_AttackCoolDown <= 0)
            {
                // Debug.Log("LastVisible -> Attack");
                currentEnemyStateAction = BossStateAction.Attack;
                return;
            }
            if (DistanceEnemyToPlayer < DISENGAGE_DISTANCE && 
            IsPlayerAround == true && 
            DistanceEnemyToPlayer >= CHASE_MIN_DISTANCE)
            {
                // Debug.Log("LastVisible -> Chase");
                currentEnemyStateAction = BossStateAction.Chase;
                return;
            }

            // Debug.Log("K nhảy vào state nào cả thì nhảy vào patrol");
            // Debug.Log("LastVisible -> Patrol");
            currentEnemyStateAction = BossStateAction.Patrol; // safe state
            return;
        }
    }
    
    private void TriggerBossLastDieFrameHandle(Sprite[] sprites)
    {
        if(sprites == bossAnimation.DeathSprites)
        {
            isDied = true;
            Destroy(gameObject);
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
        // Debug.Log("? -> Hurt");
        currentEnemyStateAction = BossStateAction.Hurt;
    }
    
    private void TriggerDieWhenHealthAsZero()
    {
        if(currentEnemyStateAction == BossStateAction.Death)
        {
            return;
        }
        // Debug.Log("? -> Death");
        currentEnemyStateAction = BossStateAction.Death;

        // set something when enemy is died
        // bossPathFindingMovement.StopMovingPhysicalHandler();
        SetUpWhenBossDie();
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

    // ==================== SUPPORT FUNCTION =====================
    private void Immediately_timer_AttackCoolDownAsZero()
    {
        timer_AttackCoolDown = 0;
    }
    
    private void DeclineTimerAttackCoolDown()
    {
        if(timer_AttackCoolDown > 0)
            timer_AttackCoolDown -= Time.deltaTime;
    }
    
    private void SetUpWhenBossDie()
    {
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        enemyHealthBar.gameObject.SetActive(false);
    }
    // =============================================================

    private void SetUpBossWhenPlayerDied()
    {
        if(playerMovement.GetPlayerState() == State.Die) // --> error in here  <--
        {
            currentEnemyStateAction = BossStateAction.Patrol;
            Debug.Log(currentEnemyStateAction + "-> Patrol");
            return;
        }
    }
}
