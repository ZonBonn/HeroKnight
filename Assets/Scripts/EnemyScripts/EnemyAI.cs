using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyStateAction { Patrol, Chase, Attack, ReadyToAttack, Idle, Die, Jump };
    public EnemyStateAction currentEnemyStateAction;
    private EnemyStateAction lastCheckedCurrentEnemyStateAction;
    private EnemyPathFindingMovement enemyPathFindingMovement = null;
    public Transform currentToward;
    private Transform lastCheckedCurrentToward;
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform chaseLeftPoint;
    public Transform chaseRightPoint;
    private Transform nullTransform = null; // đây sẽ là vị trí mà currentToward hướng tới khi đang trong các trạng thái khác ngoài state Patrol
    private Enemy enemy;
    private EnemyAnimation enemyAnimation;
    public LayerMask playerLayerMask;
    private float idleTimer = 3f;
    private float m_IdleTimer;
    private float rTCTimer = 0.5f;
    private float m_RTCTimer;

    private bool isJumping;
    private void Awake()
    {
        enemyPathFindingMovement = gameObject.GetComponent<EnemyPathFindingMovement>();
        enemy = gameObject.GetComponent<Enemy>();
        enemyAnimation = gameObject.GetComponent<EnemyAnimation>();
    }

    private void Start()
    {
        currentEnemyStateAction = EnemyStateAction.Patrol;
        currentToward = UnityEngine.Random.Range(-1, 1) > 0 ? rightPoint : leftPoint;
        lastCheckedCurrentToward = currentToward == rightPoint ? leftPoint : rightPoint;
        enemyAnimation.OnChangeEachFrames += EnemyLastAttackFrameHandler;
        enemyAnimation.OnChangeEachFrames += EnemyLastJumpFrameHandler;
    }

    private void Update()
    {
        if(currentEnemyStateAction == EnemyStateAction.Die) return;

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
        }

        // enemyAnimation.AnimationHandler(currentEnemyStateAction); // dòng này có cũng được không có cũng được vì case nào sẽ chạy animation đó rồi mà

        if (lastCheckedCurrentEnemyStateAction != currentEnemyStateAction)
        {
            Debug.Log(currentEnemyStateAction);
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

        if (Vector3.Distance(gameObject.transform.position, currentToward.position) <= 0.6f)
        {
            currentEnemyStateAction = EnemyStateAction.Idle;
            m_IdleTimer = idleTimer;
            // Debug.Log("Đổi hướng");
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
        
        if(IsSearchedPlayerAround() == true) // alway finds player in here <-- SearchingPlayerAround();
        {
            currentToward = nullTransform; // đổi hướng khi chuyển trạng thái cho đỡ phải tốn chi phí, lại còn tạo ra random hướng của mỗi enemy
            currentEnemyStateAction = EnemyStateAction.Chase;
            return;
        }
        
        if (enemyPathFindingMovement.IsHole() || Input.GetKeyDown(KeyCode.L) || enemyPathFindingMovement.IfCanJumpOverTheInFrontWall())
        {
            currentToward = nullTransform;
            Debug.Log("Is Hole:" + enemyPathFindingMovement.IsHole() + "  ;;;   IfCanJumpOverTheInFrontWall:" + enemyPathFindingMovement.IfCanJumpOverTheInFrontWall());
            currentEnemyStateAction = EnemyStateAction.Jump;
            isJumping = true;
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
        enemyPathFindingMovement.StopMovingPhysicalHandler();
        Immediately_m_RTCTimer();
        if (m_IdleTimer > 0)
        {
            m_IdleTimer -= Time.deltaTime;
        }
        else
        {
            currentEnemyStateAction = EnemyStateAction.Patrol;
            return;
        }
        if(IsSearchedPlayerAround() == true)// alway finds player in here <-- SearchingPlayerAround();
        {
            currentEnemyStateAction = EnemyStateAction.Chase;
            return;
        } 
    }

    private void ChaseActionHandler()
    {
        AlwayTowardToPlayer();
        Immediately_m_RTCTimer();
        if (Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) <= 1.5f && IsSearchedPlayerAround() == true)
        {
            currentEnemyStateAction = EnemyStateAction.ReadyToAttack;
            return;
        }
        else
        {
            enemyPathFindingMovement.MoveTo(Player.Instance.GetPlayerPosition());
            // trong lúc đang đuổi theo nên check hố chướng ngại vật các thứ v.v tại đây --> IN HERE <-- tại đây
        }

        if (Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) >= 4f && IsSearchedPlayerAround() == false)
        {
            currentEnemyStateAction = EnemyStateAction.Patrol;
            return;
        }
        if (enemyPathFindingMovement.IsHole() || Input.GetKeyDown(KeyCode.L) || enemyPathFindingMovement.IfCanJumpOverTheInFrontWall())
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
        if (IsSearchedPlayerAround() == false && Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) <= 1f)
        {
            // while attacking player and dont see the player
            enemyPathFindingMovement.MoveTo(Player.Instance.GetPlayerPosition());
        }
    }

    private void ReadyToAttackActionHandler()
    {
        m_RTCTimer -= Time.deltaTime;
        // viết hàm luôn luôn nhìn về hướng player khi đang ở trạng thái readyTOAttack tại đây
        AlwayTowardToPlayer();
        enemyPathFindingMovement.StopMovingPhysicalHandler();
        if (Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) <= 1f && IsSearchedPlayerAround() == true && m_RTCTimer <= 0)
        {
            currentEnemyStateAction = EnemyStateAction.Attack;
            return;
        }
        if (Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) >= 4f && IsSearchedPlayerAround() == false)
        {
            currentEnemyStateAction = EnemyStateAction.Patrol;
            return;
        }
        if (Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) < 4f && 
        IsSearchedPlayerAround() == true && 
        Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) >= 2f)
        {
            currentEnemyStateAction = EnemyStateAction.Chase;
            return;
        }
    }

    private void JumpActionHandler()
    {
        if(enemyPathFindingMovement.IsGrounded() == true && isJumping == false) // đã tiếp đất thì mới được chuyển trạng thái 
        {
            if (Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) >= 4f && IsSearchedPlayerAround() == false)
            {
                currentEnemyStateAction = EnemyStateAction.Patrol;
                return;
            }
            if (Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) <= 1.5f && IsSearchedPlayerAround() == true)
            {
                currentEnemyStateAction = EnemyStateAction.ReadyToAttack;
                return;
            }
            if (Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) < 4f && 
            IsSearchedPlayerAround() == true && 
            Vector3.Distance(gameObject.transform.position, Player.Instance.GetPlayerPosition()) >= 2f)
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
    // =============================================================


    //======== HANDLER ANIMATION BY EACH FRAMES =========
    private void EnemyLastAttackFrameHandler(int idxFrame, Sprite[] sprites)
    {
        if (sprites == enemyAnimation.AttackSprites && idxFrame == enemyAnimation.AttackSprites.Length - 1)
        {
            m_RTCTimer = rTCTimer;
        }
    }
    
    private void EnemyLastJumpFrameHandler(int idxFrame, Sprite[] sprites)
    {
        if(sprites == enemyAnimation.JumpSprites && idxFrame == enemyAnimation.JumpSprites.Length)
        {
            isJumping = false;
        }
    }
    // ===========================================================
    


    // ========= SUPPORTING FUNCTION ========
    private void Immediately_m_RTCTimer()
    {
        m_RTCTimer = 0;
    }

    private void AlwayTowardToPlayer(){
        Vector2 DirToTarget = Player.Instance.GetPlayerPosition() - gameObject.transform.position;
                
        // handler Visual Direction and Flip Direction
        float tmpDirToTargetX = DirToTarget.x;
        if (-0.55f <= tmpDirToTargetX && tmpDirToTargetX <= 0.55f)
        {
            tmpDirToTargetX = enemyPathFindingMovement.currentVisualDir; // default value if the amount too small
        }
        
        int moveDirX = Math.Sign(tmpDirToTargetX);
        
        enemyPathFindingMovement.currentVisualDir = enemyPathFindingMovement.LeftOrRightPlatformer(moveDirX) == true ? 1 : -1;
    }

    private bool IsSearchedPlayerAround() // continue your work in here --> IN HERE <--
    {
        Vector2 VisualDir = enemyPathFindingMovement.currentVisualDir == -1 ? Vector2.left : Vector2.right;
        Vector3 maxDistanceVisualPoint = VisualDir == Vector2.left ? chaseLeftPoint.position : chaseRightPoint.position;
        // đây sẽ là hai đểm ChaseWaypointA hoặc ChaseWaypointB
        float DistanceVisual = Vector3.Distance(gameObject.transform.position, maxDistanceVisualPoint);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(gameObject.transform.position, VisualDir, DistanceVisual, playerLayerMask);
        Debug.DrawLine(gameObject.transform.position, maxDistanceVisualPoint, Color.darkBlue, 0.1f);
        if (raycastHit2D.collider != null)
        {
            return true;
        }
        return false;
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
