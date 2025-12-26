using System.Diagnostics;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum State { Null, Idle, Run, Attack1, Attack2, Attack3, Die, Jump, Roll, ClimbingOnWall, ClimbingActionWall, Fall, Hit, BlockIdle, BlockHit }

// Idle, Run, Jump == Normal ???
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float MOVE_SPEED = 5f;
    private float ROLL_SPEED;
    private const float JUMP_FORCE = 14f;
    private Rigidbody2D rb2D;
    private Vector3 moveDir;
    private CapsuleCollider2D capsuleCollider2D;
    [SerializeField] private LayerMask platFormLayerMask;
    [SerializeField] private LayerMask wallLayerMask;
    private State playerState;
    private State lastCheckedPlayerState;
    private PlayerAnimation playerAnimation;
    private SpriteRenderer spriteRenderer;

    // combo attacks
    private bool canQueueNextAttack;
    private bool canMoveToAttack2;
    private bool canMoveToAttack3;

    // Flag thời gian kéo dài hiệu lực vật lý
    bool isPressedSpace = false; // <=> == isJumping
    // đánh dấu nhảy đúng một frame rồi thôi tắt nó thành false luôn
    // bool isPressedF = false; // <=> == isRolling
    bool isJumpFromWall = false;
    
    private float LastMoveDir;
    private float DefaultGravityScale = 3.5f;
    private bool TouchedWallAlwaysFalse;
    private float TouchedWallAlwaysFalseTimer;
    private int WallDirX;

    private PlayerAttack playerAttack;

    private int currentPlayerVisualDirection = +1;

    private bool IsOnGroundedVar;
    private bool IsTouchedWallVar;
    private bool IsEnoughManaForNormalAttackVar;

    private PlayerHealthStaminaHandler playerHealthStaminaHandler;
    private PlayerHealthSystem playerHealthSystem;

    private float KNOCK_BACK_HORIZONTAL_FORCE = 5f;
    private const float KNOCK_BACK_VERTICAL_FORCE = 5f;
    
    private bool IsOnGroundedVarFixedUpdate;
    private bool IsTouchedWallVarFixedUpdate;


    private void Awake()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        capsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        playerAttack = gameObject.GetComponent<PlayerAttack>();
        playerHealthStaminaHandler = gameObject.GetComponent<PlayerHealthStaminaHandler>();
        playerHealthSystem = playerHealthStaminaHandler.GetPlayerHealthSystem();
        playerAnimation = gameObject.GetComponent<PlayerAnimation>();
        playerState = State.Idle;
        spriteRenderer = playerAnimation.GetSpriteRenderer();
        // animation trigger
        playerAnimation.OnChangeLastFrames += OnEndOfAttackSprites;
        playerAnimation.OnChangeLastFrames += OnEndOfRollSprites;
        playerAnimation.OnChangeLastFrames += OnEndOfJumpSprites;
        playerAnimation.OnChangeLastFrames += OnEndOfFallSprites;
        playerAnimation.OnChangeLastFrames += OnEndOfRunSprites;
        playerAnimation.OnChangeEachFrames += HandlerAttackFrames;
        playerAnimation.OnChangeLastFrames += OnEndOfHurtSprites;
        playerAnimation.OnChangeLastFrames += OnEndOfBlockHitedFrame;
        //health stamina trigger
        playerHealthSystem.OnTriggerPlayerHealthChange += TriggerHurtPlayerWhenHealthChange;
        playerHealthSystem.OnTriggerPlayerHealthChange += TriggerDieWhenPlayerHealthAsZero;
        // block idle trigger
        playerHealthStaminaHandler.OnBlockIdleIsHited += TriggerWhenBlockIdleHited;

        
    }

    private void Update()
    {
        if (playerState == State.Die)
        {
            // recovery player in here ????
            
            return;
        }
    
        // optimize
        IsOnGroundedVar = IsGrounded();
        IsTouchedWallVar = IsTouchedWall();
        IsEnoughManaForNormalAttackVar = playerAttack.CanEnoughManaForNormalAttack();

        // UnityEngine.Debug.Log("IsGrounded:" + IsOnGroundedVar); 
        
        // INPUTS HANDLER:
        float moveX = 0f;
        float moveY = 0f;

        switch (playerState)
        {
            case State.Idle:
                // set action
                if (Input.GetKeyDown(KeyCode.Space) && IsOnGroundedVar == true)
                {
                    isPressedSpace = true;
                    break;
                }

                // set state
                if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && IsOnGroundedVar == true)
                {
                    playerState = State.Run;
                    break;
                }

                if (IsOnGroundedVar == false)
                {
                    playerState = State.Jump;
                    break;
                }

                if (Input.GetMouseButtonDown(0) && IsEnoughManaForNormalAttackVar == true)
                {
                    playerState = State.Attack1;
                    break;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    ROLL_SPEED = 10f;
                    playerState = State.Roll;
                    break;
                }
                if (Input.GetMouseButton(1) && IsOnGroundedVar == true)
                {
                    playerState = State.BlockIdle;
                    break;
                }
                break;

            case State.Run:
                // set action
                if (Input.GetKey(KeyCode.A))
                {
                    moveX = -1f;
                    LastMoveDir = -1f;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    moveX = +1f;
                    LastMoveDir = +1f;
                }

                if (Input.GetKeyDown(KeyCode.Space) && IsOnGroundedVar == true)
                {
                    isPressedSpace = true;
                    break;
                }

                // set state (KHÔNG CẦN ƯU TIÊN VÌ CÁI NÀO IF CUỐI CÙNG SẼ ĐƯỢC ƯU TIÊN MÀ :))
                if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && IsOnGroundedVar == true)
                {
                    playerState = State.Idle;
                    break;
                }

                if (IsOnGroundedVar == false)
                {
                    playerState = State.Jump;
                    break;
                }

                if (Input.GetMouseButtonDown(0) && IsEnoughManaForNormalAttackVar == true)
                {
                    if (canMoveToAttack2 == true)
                    {
                        playerState = State.Attack2;
                        canMoveToAttack2 = false;
                        break;
                    }
                    else if (canMoveToAttack3 == true)
                    {
                        playerState = State.Attack3;
                        canMoveToAttack3 = false;
                        break;
                    }
                    playerState = State.Attack1;
                    break;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    ROLL_SPEED = 10f;
                    playerState = State.Roll;
                    break;
                }
                if (Input.GetMouseButton(1) && IsOnGroundedVar == true)
                {
                    playerState = State.BlockIdle;
                    break;
                }
                break;

            case State.Jump:
                // khi đang ở trạng thái nhảy
                // set state
                if (IsOnGroundedVar == true) // ngay khi chạm đất
                {
                    // set state
                    if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && IsOnGroundedVar == true)
                    {
                        playerState = State.Idle;
                        break;
                    }

                    if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && IsOnGroundedVar == true)
                    {
                        playerState = State.Run;
                        break;
                    }

                    if (Input.GetMouseButtonDown(0) && IsEnoughManaForNormalAttackVar == true)
                    {
                        if (canMoveToAttack2 == true)
                        {
                            playerState = State.Attack2;
                            canMoveToAttack2 = false;
                            break;
                        }
                        else if (canMoveToAttack3 == true)
                        {
                            playerState = State.Attack3;
                            canMoveToAttack3 = false;
                            break;
                        }
                        playerState = State.Attack1;
                        break;
                    }

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        ROLL_SPEED = 10f;
                        playerState = State.Roll;
                        break;
                    }
                }
                else if (IsOnGroundedVar == false) // khi ở trạng thái jump trên không, vì 
                // trên không vẫn di chuyển được nên phải cộng thêm giá trị khi di chuyển trên không nữa, còn chuyển trạng thái thì không cần
                {
                    // set action
                    if (Input.GetKey(KeyCode.A))
                    {
                        moveX = -1f;
                        LastMoveDir = -1f;
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        moveX = +1f;
                        LastMoveDir = +1f;
                    }

                    // set state while on air
                    if (Input.GetMouseButtonDown(0) && IsEnoughManaForNormalAttackVar == true)
                    {
                        if (canMoveToAttack2 == true)
                        {
                            playerState = State.Attack2;
                            canMoveToAttack2 = false;
                            break;
                        }
                        else if (canMoveToAttack3 == true)
                        {
                            playerState = State.Attack3;
                            canMoveToAttack3 = false;
                            break;
                        }
                        playerState = State.Attack1;
                        break;
                    }

                    if (Input.GetKey(KeyCode.W) && IsTouchedWallVar == true)
                    {
                        playerState = State.ClimbingOnWall;
                        rb2D.gravityScale = 0f; // không cho rơi xuống khi đang leo
                        break;
                    }
                }
                break;

            case State.Fall:

                if (IsOnGroundedVar == true) // ngay khi chạm đất
                {
                    // set state
                    if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && IsOnGroundedVar == true)
                    {
                        playerState = State.Idle;
                        break;
                    }

                    if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && IsOnGroundedVar == true)
                    {
                        playerState = State.Run;
                        break;
                    }

                    if (Input.GetMouseButtonDown(0) && IsEnoughManaForNormalAttackVar == true)
                    {
                        playerState = State.Attack1;
                        break;
                    }

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        ROLL_SPEED = 10f;
                        playerState = State.Roll;
                        break;
                    }
                }
                else if (IsOnGroundedVar == false) // khi ở trạng thái fall trên không, vì 
                // trên không vẫn di chuyển được nên phải cộng thêm giá trị khi di chuyển trên không nữa, còn chuyển trạng thái thì không cần
                {
                    // set action
                    if (Input.GetKey(KeyCode.A))
                    {
                        moveX = -1f;
                        LastMoveDir = -1f;
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        moveX = +1f;
                        LastMoveDir = +1f;
                    }

                    // set state while on air
                    if (Input.GetMouseButtonDown(0) && IsEnoughManaForNormalAttackVar == true)
                    {
                        playerState = State.Attack1;
                        break;
                    }

                    if (Input.GetKey(KeyCode.W) && IsTouchedWallVar == true)
                    {
                        playerState = State.ClimbingOnWall;
                        rb2D.gravityScale = 0f; // không cho rơi xuống khi đang leo
                        break;
                    }
                }
                break;

            case State.Attack1:
                // do phải chạy hết aniamtion tới frame cuối thì mới được chuyển state => xử lý riêng ở delegate Action
                if (Input.GetMouseButtonDown(0) && IsEnoughManaForNormalAttackVar == true) // Clicked mouse during attack1 state
                {
                    if (canQueueNextAttack == true && playerState == State.Attack1)
                    {
                        canMoveToAttack2 = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.F) && IsOnGroundedVar == true)
                {
                    ROLL_SPEED = 10f;
                    playerState = State.Roll;
                    break;
                }
                break;

            case State.Attack2:
                if (Input.GetMouseButtonDown(0) && IsEnoughManaForNormalAttackVar == true)
                {
                    if (canQueueNextAttack == true && playerState == State.Attack2)
                    {
                        canMoveToAttack3 = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.F) && IsOnGroundedVar == true)
                {
                    ROLL_SPEED = 10f;
                    playerState = State.Roll;
                    break;
                }
                break;

            case State.Attack3:
                if (Input.GetKeyDown(KeyCode.F) && IsOnGroundedVar == true)
                {
                    ROLL_SPEED = 10f;
                    playerState = State.Roll;
                    break;
                }
                break;

            case State.Roll:
                float rollSpeedDropMultiplier = 0.3f;
                ROLL_SPEED -= ROLL_SPEED * rollSpeedDropMultiplier * Time.deltaTime;
                float rollSpeedMinimum = 5f; // == MOVE_SPEED

                if (ROLL_SPEED <= rollSpeedMinimum) // đây là 1 trong 2 cách chuyển từ state roll -> state khác: chờ tới khi tốc độ lăn về với tốc độ MOVE_SPEED
                {
                    if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    {
                        playerState = State.Idle;
                        break;
                    }
                        
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        playerState = State.Run;
                        break;
                    }
                        
                    if (Input.GetKey(KeyCode.Space))
                    {
                        playerState = State.Jump;
                        break;
                    }
                        
                    if (Input.GetMouseButtonDown(0))
                    {
                        playerState = State.Attack1;
                        break;
                    }
                        
                }

                // về sau thì khi roll sẽ không nhận damage thì sẽ thêm các chức năng khi đang rolling TẠI ĐÂY
                break;

            case State.ClimbingOnWall:
                if (!Input.GetKey(KeyCode.W)) // chừng nào không còn ấn nút nữa
                {
                    rb2D.gravityScale = DefaultGravityScale;
                    playerState = State.Jump; // lúc thực hiện state này chỉ khi đang nhảy => lúc không còn thực hiện thì vẫn đang là State Jump => quay trở lại State Jump
                    break;
                }
                else if (Input.GetKey(KeyCode.W)) // chừng nào còn ấn nút W để duy trì trạng thái leo tường
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        // if (!IsTouchedWallVar)
                        // {
                        //     rb2D.gravityScale = DefaultGravityScale;
                        //     playerState = State.Jump;
                        //     break;
                        // }

                        if (Input.GetKey(KeyCode.A) && (WallDirX == 1 || LastMoveDir > 0))
                        {
                            // moveX = -1f;
                            // LastMoveDir = -1f;
                            // không cần cập nhật last move vì khi state = Jump nó sẽ tự cập nhật
                            isPressedSpace = true;
                            playerState = State.Jump;
                            rb2D.gravityScale = DefaultGravityScale;
                            TouchedWallAlwaysFalseTimer = 0.15f; // cho thêm một khoảng thời gian rất ngắn tại đây để IsTochedWall luôn luôn == false
                            isJumpFromWall = true;
                            break;
                        }

                        if (Input.GetKey(KeyCode.D) && (WallDirX == -1 || LastMoveDir < 0))
                        {
                            isPressedSpace = true;
                            playerState = State.Jump;
                            rb2D.gravityScale = DefaultGravityScale;
                            TouchedWallAlwaysFalseTimer = 0.15f;
                            isJumpFromWall = true;
                            break;
                        }
                    }
                }
                break;

            case State.Die:
                
                break;

            case State.Hit:
                // do nothing when player got hited
                break;

            case State.BlockIdle:
                if (!Input.GetMouseButton(1))
                {
                    if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && IsOnGroundedVar == true)
                    {
                        playerState = State.Idle;
                        break;
                    }

                    if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && IsOnGroundedVar == true)
                    {
                        playerState = State.Run;
                        break;
                    }
                }
                break;

            case State.BlockHit:
                float knockBackSpeedDropMultiplier = 0.3f;// đây là 1 trong 2 cách chuyển từ state BlockHit -> state khác: chờ tới khi tốc độ KNOCK_BACK_HORIZONTAL_FORCE về với tốc độ knockBackSpeedMinimum
                KNOCK_BACK_HORIZONTAL_FORCE = KNOCK_BACK_HORIZONTAL_FORCE - (knockBackSpeedDropMultiplier * Time.deltaTime);
                float knockBackSpeedMinimum = 5f; 
                if(KNOCK_BACK_HORIZONTAL_FORCE <= knockBackSpeedMinimum)
                {
                    // chuyển về tốc độ di chuyển bình thường chứ không phải là chuyển state ? chuyển state
                    CanTransformIdle();
                    CanTransformBlockIdle(IsOnGroundedVar);
                    CanTransformJump();
                    CanTransformRoll();
                    CanTransformRun();
                }
                break;
        }

        moveDir = new Vector2(moveX, moveY).normalized;

        // UnityEngine.Debug.Log(playerState);
        // if(lastCheckedPlayerState != playerState)
        // {
        //     UnityEngine.Debug.Log(playerState);
        //     lastCheckedPlayerState = playerState;
        // }

        // ANIMATIONS HANDLER:
        playerAnimation.AnimationHandler(playerState); // cho nó tự chuyển animation theo state, hợp lý đấy đỡ phải gọi gây rối

        // FlipSPrite Handler;
        FlipDir();
    }

    private void FixedUpdate()
    {
        if (playerState == State.Die)
        {
            rb2D.linearVelocity = new Vector2(0, 0);
            return;
        }

        IsOnGroundedVarFixedUpdate = IsGrounded();
        IsTouchedWallVarFixedUpdate = IsTouchedWall();

        // PHYSICS HANDLER:
        if (playerState == State.Idle || playerState == State.Run || playerState == State.Jump || playerState == State.Fall) // Normal State == Idle, Run, Jump
        {
            JumpPhysicsHandler();
            MovementPhysicsHandler();
        }
        else if (playerState == State.Attack1 || playerState == State.Attack2 || playerState == State.Attack3)
        {
            // thực hiện attack
            // handler physic attack in here --> in here <--
            if (IsOnGroundedVar == true)
            {
                rb2D.linearVelocity = new Vector2(0, rb2D.linearVelocity.y);
            }
            else if (IsOnGroundedVar == false)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y);
            }
        }
        else if (playerState == State.Roll)
        {
            float RollDir = LastMoveDir;
            rb2D.linearVelocity = new Vector2(ROLL_SPEED * RollDir, rb2D.linearVelocity.y);
        }
        else if (playerState == State.ClimbingOnWall && IsTouchedWallVar == true)
        {
            rb2D.linearVelocity = new Vector2(0, 0);
        }
        else if (playerState == State.BlockIdle)
        {
            rb2D.linearVelocity = new Vector2(0, 0);
        }
        else if(playerState == State.Hit)
        {
            rb2D.linearVelocity = new Vector2(0, 0);
        }
        else if(playerState == State.BlockHit)
        {
            rb2D.linearVelocity = new Vector2(KNOCK_BACK_HORIZONTAL_FORCE * (currentPlayerVisualDirection * -1), KNOCK_BACK_VERTICAL_FORCE);
        }
        else if(playerState == State.Die)
        {
            
        }
    }

// ====================== PHYSICAL HANDLERS FUNCTION ========================
    private void MovementPhysicsHandler()
    {
        SomeControllOnAir();
    }

    private void JumpPhysicsHandler()
    {
        // optimize
        // bool IsOnGroundedVar = IsGrounded();
        // bool IsTouchedWallVar = IsTouchedWall();
        

        if (isPressedSpace == true && (IsOnGroundedVarFixedUpdate == true || IsTouchedWallVarFixedUpdate == true || isJumpFromWall == true)) // nếu có thể nhảy: 2 mức nhảy, nhảy khi ở tường sẽ cao hơn ở đất
        {
            if (IsOnGroundedVarFixedUpdate == true)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, JUMP_FORCE); // khi nhảy vẫn giữ lại vận tốc rb2D.linearVelocity.x trước đó => không bị kẹt cứng một chỗ
                isPressedSpace = false;
            }
            else // == else if(IsTouchedWallVarFixedUpdate == true)
            {
                // UnityEngine.Debug.Log("Nhảy ra từ tường");
                float JUMP_FORCE_ON_WALL = JUMP_FORCE + 4;
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, JUMP_FORCE_ON_WALL); // khi nhảy vẫn giữ lại vận tốc rb2D.linearVelocity.x trước đó => không bị kẹt cứng một chỗ
                isPressedSpace = false;
                isJumpFromWall = false;
            }
        }
    }

    private void SomeControllOnAir()
    {
        // optimize
        // bool IsOnGroundedVar = IsGrounded();

        float maxSpeedOnAir = 6f;
        float midAirControlSpeed = 10f;
        
        if (moveDir.x != 0) // di chuyển
        {
            if (IsOnGroundedVarFixedUpdate == true) // dưới đất
            {
                rb2D.linearVelocity = new Vector2(moveDir.x * MOVE_SPEED, rb2D.linearVelocity.y);
            }
            else // trên không
            {
                if (moveDir.x > 0) // == 1f ?
                {
                    rb2D.linearVelocity += new Vector2(midAirControlSpeed * Time.deltaTime, 0f); // tăng dần đều mỗi giây tăng được midAirControlSpeed tốc độ
                    rb2D.linearVelocity = new Vector2(Mathf.Clamp(rb2D.linearVelocity.x, -maxSpeedOnAir, +maxSpeedOnAir), rb2D.linearVelocity.y);
                }

                if (moveDir.x < 0) // == -1f ?
                {
                    rb2D.linearVelocity += new Vector2(-midAirControlSpeed * Time.deltaTime, 0f);
                    rb2D.linearVelocity = new Vector2(Mathf.Clamp(rb2D.linearVelocity.x, -maxSpeedOnAir, +maxSpeedOnAir), rb2D.linearVelocity.y);
                }
            }
        }
        else // đứng yên
        {
            if (IsOnGroundedVarFixedUpdate == true) // dưới đất
            {
                rb2D.linearVelocity = new Vector2(moveDir.x * MOVE_SPEED, rb2D.linearVelocity.y);
            }
            else // trên không
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y);
            }
        }
    }

    public void KnockBackPhysicsHandler()
    {
        
        rb2D.linearVelocity = new Vector2(KNOCK_BACK_HORIZONTAL_FORCE * (currentPlayerVisualDirection * -1f), KNOCK_BACK_VERTICAL_FORCE);
    }
// ===============================================================

    
// ================== FRAMES HANDLERS FUNCTION ====================
    private void OnEndOfAttackSprites(Sprite[] currentSprite) // được chạy khi cuối cùng của frame của attack
    {
        // optimize
        // bool IsOnGroundedVar = IsGrounded();
        bool IsEnoughManaForNormalAttackVar = playerAttack.CanEnoughManaForNormalAttack();
        
        // set state
        if (playerState == State.Attack1)
        {
            if (IsOnGroundedVar == true) // attack on ground
            {
                if (canMoveToAttack2 == true && IsEnoughManaForNormalAttackVar == true)
                {
                    playerState = State.Attack2;
                    canMoveToAttack2 = false;
                    return;
                }
                else if (canMoveToAttack2 == false)
                {
                    if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    {
                        playerState = State.Idle;
                        return;
                    }

                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        playerState = State.Run;
                        return;
                    }

                    if (Input.GetKeyDown(KeyCode.Space) && IsOnGroundedVar == true)
                    {
                        isPressedSpace = true;
                        return;
                    }
                }
            }
            else if (IsOnGroundedVar == false) // attack on air
            {
                if (canMoveToAttack2 == true && IsEnoughManaForNormalAttackVar == true)
                {
                    playerState = State.Attack2;
                    canMoveToAttack2 = false;
                    return;
                }
                else if (canMoveToAttack2 == false)
                {
                    playerState = State.Jump;
                    return;
                }
            }
        }
        else if (playerState == State.Attack2)
        {
            if (IsOnGroundedVar == true) // attack on ground
            {
                if (canMoveToAttack3 == true && IsEnoughManaForNormalAttackVar == true)
                {
                    playerState = State.Attack3;
                    canMoveToAttack3 = false;
                    return;
                }
                else if (canMoveToAttack3 == false)
                {
                    if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    {
                        playerState = State.Idle;
                        return;
                    }

                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        playerState = State.Run;
                        return;
                    }

                    if (Input.GetKeyDown(KeyCode.Space) && IsOnGroundedVar == true)
                    {
                        isPressedSpace = true;
                        return;
                    }
                }
            }
            else if (IsOnGroundedVar == false)
            {
                if (canMoveToAttack3 == true && IsEnoughManaForNormalAttackVar == true)
                {
                    playerState = State.Attack3;
                    canMoveToAttack3 = false;
                    return;
                }
                else if (canMoveToAttack3 == false)
                {
                    playerState = State.Jump;
                    return;
                }
            }
        }
        else if (playerState == State.Attack3)
        {
            if (IsOnGroundedVar == true) // attack on ground
            {
                if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    playerState = State.Idle;
                    return;
                }

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    playerState = State.Run;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Space) && IsOnGroundedVar == true)
                {
                    isPressedSpace = true;
                    return;
                }
            }
            else if (IsOnGroundedVar == false) // attack on air
            {
                playerState = State.Jump;
                return;
            }
        }
    }

    private void OnEndOfRollSprites(Sprite[] currentSprite) // đây là 1 trong 2 cách chuyển từ state roll -> state khác: đợi tới frame cuối của roll
    {
        if (playerState == State.Roll) // on the ground
        // check playerState == Roll vì Invoke được gọi tất cả hàm đăng ký ngay cả hàm này nhưng chỉ hoạt động hàm này khi đang trạng thái Roll thôi
        {
            // optimize
            bool IsOnGroundedVar = IsGrounded();

            if (IsOnGroundedVar == true)
            {
                if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    playerState = State.Idle;
                }

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    playerState = State.Run;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isPressedSpace = true;
                }
            }
            else if (IsOnGroundedVar == false && playerState == State.Roll) // on air
            {
                playerState = State.Jump;
            }
        }
    }

    private void OnEndOfJumpSprites(Sprite[] currentSprite)
    {
        if (playerState == State.Jump && IsGrounded() == false)
        {
            playerState = State.Fall;
        }
         // có thể cho nó reset về attack 1 nhanh hơn nhưng ME thích cuối :D
        canMoveToAttack2 = false; 
        canMoveToAttack3 = false;
    }

    private void OnEndOfRunSprites(Sprite[] currentSprite)
    {
        // có thể cho nó reset về attack 1 nhanh hơn nhưng ME thích cuối :D
        canMoveToAttack2 = false;
        canMoveToAttack3 = false;
    }

    private void OnEndOfFallSprites(Sprite[] currentSprite)
    {
        // có thể cho nó reset về attack 1 nhanh hơn nhưng ME thích cuối :D
        canMoveToAttack2 = false;
        canMoveToAttack3 = false;
    }

    private void HandlerAttackFrames(int idxFrame, Sprite[] currentSprite) // một số frame chuyển canQueueAttack = true, rồi check nếu trong khoảng frame đấy nếu bấm chuột trái và canQueueAttack == true thì next combo attack
    {
        
        if (currentSprite == playerAnimation.Attack1Sprites || currentSprite == playerAnimation.Attack2Sprites || currentSprite == playerAnimation.Attack3Sprites)
        {
            // UnityEngine.Debug.Log("idx Frame:" + idxFrame + " currentSprite:" + currentSprite);
            if (0 < idxFrame && idxFrame < 5) // hard code phần attack nhưng thôi kệ
            {
                canQueueNextAttack = true;
            }
            else if (idxFrame < 1 || idxFrame >= 5)
            {
                canQueueNextAttack = false;
            }

            if(idxFrame == 5)
            {
                // UnityEngine.Debug.Log("created attack point");
                playerAttack.CreatePointAttack(currentSprite);
            }

            if (idxFrame == 5 && (CanTransformRun() || CanTransformRoll() || CanTransformJump()))
            {
                CanTransformRun();
                CanTransformRoll();
                CanTransformJump();
            }

        }

    }

    private void OnHandlerEachFramesHurt(int idxFrame, Sprite[] currentSprite) // bỏ mình knock back enemy thôi, chứ enemy knockback mình làm gì :)
    {
        if(currentSprite == playerAnimation.HurtSprites && idxFrame == 1)
        {
            
        }
    }

    private void OnEndOfHurtSprites(Sprite[] currentSprite)
    {
        if(currentSprite == playerAnimation.HurtSprites)
        {
            CanTransformIdle();
            CanTransformRun();
            CanTransformJump();
            CanTransformRoll();
            CanTransformBlockIdle(IsOnGroundedVar);
        }
    }

    private void OnEndOfBlockHitedFrame(Sprite[] currentSprite)
    {
        if(currentSprite == playerAnimation.BlockHitSprites)// đây là 1 trong 2 cách chuyển từ state BlockHit -> state khác: chờ tới cuối frame thì mới đổi
        {
            CanTransformIdle();
            CanTransformBlockIdle(IsOnGroundedVar);
            CanTransformJump();
            CanTransformRoll();
            CanTransformRun();
        }
    }
// ===============================================================


// ================== HEALTH AND STAMINA AND BLOCK IDLE TRIGGER HANDLERS FUNCTION ====================
    private void TriggerHurtPlayerWhenHealthChange(float currentHealth)
    {
        // if(playerHealthSystem.GetCurrentHealth() <= 0)
        // {
        //     return;
        // }
        if(playerState == State.Hit && currentHealth > 0)
        {
            return;
        }
        playerState = State.Hit; // đây là cách thay đổi playerState từ ngoài mà vẫn đảm bảo encapsulation =:D
    }

    private void TriggerWhenBlockIdleHited()
    {
        KNOCK_BACK_HORIZONTAL_FORCE = 7f;
        playerState = State.BlockHit;
    }

    private void TriggerDieWhenPlayerHealthAsZero(float currentHealth)
    {
        if(playerState == State.Die)
        {
            return;
        }
        if(currentHealth <= 0)
        {
            CustomizePlayerDeathSetting();
        }
        
    }
// ====================================================================================

    
// ====================== SENSOR FUNCTION ========================
    private bool IsGrounded()
    {
        WallDirX = 0;
        // RaycastHit2D rayCastHit2D = Physics2D.BoxCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, 0f, Vector2.down, 0.1f, platFormLayerMask);
        
        RaycastHit2D rayCastHit2D = Physics2D.Raycast(capsuleCollider2D.bounds.center, Vector3.down, capsuleCollider2D.size.y * 0.5f + 0.05f, platFormLayerMask);
        if (rayCastHit2D.collider != null)
        {
            return true;
        }
        return false;
    }

    private bool IsTouchedWall()
    {
        if (TouchedWallAlwaysFalseTimer > 0) // ignore touched wall as soon as jump out from wall by timer
        {
            TouchedWallAlwaysFalseTimer -= Time.deltaTime;
            return false;
        }

        float rayLength = (capsuleCollider2D.size.x * .5f) + 0.01f;
        Vector2 origin = capsuleCollider2D.bounds.center;
        bool leftHit = Physics2D.Raycast(origin, Vector2.left, rayLength, wallLayerMask);
        bool rightHit = Physics2D.Raycast(origin, Vector2.right, rayLength, wallLayerMask);
        // UnityEngine.Debug.DrawRay(origin, Vector2.left * rayLength, Color.red);
        // UnityEngine.Debug.DrawRay(origin, Vector2.right * rayLength, Color.red);

        if (leftHit == true && rightHit == false)
        {
            WallDirX = -1;
            LastMoveDir = -1f;
            return true;
        }

        if (rightHit == true && leftHit == false)
        {
            WallDirX = +1;
            LastMoveDir = +1f;
            return true;
        }

        // UnityEngine.Debug.DrawRay(origin, Vector2.right * rayLength, Color.red);
        WallDirX = 0;
        return false;
    }
// ===============================================================


// ====================== SUPPORTING FUNCTION ========================
    private bool CanTransformIdle()
    {
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            playerState = State.Idle;
            return true;
        }
        return false;
    }

    private bool CanTransformRun()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            playerState = State.Run;
            return true;
        }
        return false;
    }

    private bool CanTransformJump()
    {
        if (Input.GetKey(KeyCode.Space) && IsOnGroundedVar == true)
        {
            isPressedSpace = true;
            return true;
        }
        return false;
    }

    private bool CanTransformRoll()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ROLL_SPEED = 10f;
            playerState = State.Roll;
            return true;
        }
        return false;
    }

    private bool CanTransformBlockIdle(bool IsGrounded)
    {
        if (Input.GetMouseButton(1) && IsGrounded == true)
        {
            playerState = State.BlockIdle;
            return true;
        }
        return false;
    }
// ===============================================================


// ====================== ORTHERS FUNCTION ========================
    private void FlipDir()
    {
        if (playerState == State.ClimbingOnWall && WallDirX != 0) // special sprites handler
        {
            if (WallDirX == 1)
            {
                spriteRenderer.flipX = false;
                currentPlayerVisualDirection = -1;

            }
            else if (WallDirX == -1)
            {
                spriteRenderer.flipX = true;
                currentPlayerVisualDirection = +1;
            }
        }
        else
        {
            if (moveDir.x > 0)
            {
                spriteRenderer.flipX = false;
                currentPlayerVisualDirection = +1; // phai
            }
            else if (moveDir.x < 0)
            {
                spriteRenderer.flipX = true;
                currentPlayerVisualDirection = -1; // trai
            }
        }
    }

    public State GetPlayerState()
    {
        return playerState;
    }

    public int GetPlayerVisualDirection()
    {
        return currentPlayerVisualDirection;
    }

    public void SetPlayerState(State playerState) // hàm này chỉ dùng cho class PlayerSupportTestTool chứ không được xử dụng cho bất kỳ class nào khác
    {
        this.playerState = playerState;
    }

    private void CustomizePlayerDeathSetting()
    {
        playerState = State.Die;
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        playerAnimation.AnimationHandler(playerState);
    }
// ===============================================================

}
