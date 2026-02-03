using System.Collections.Generic;
using UnityEngine;
using System;

public class BossPathFindingMovement : MonoBehaviour
{
    public const float MOVE_SPEED = 2f;
    public const float JUMP_FORCE = 14f;
    private const float PUSH_FORCE = 5.5f;
    private const float KNOCK_BACK_HORIZONTAL_FORCE = 2f;
    private const float KNOCK_BACK_VERTICAL_FORCE = 1f;
    public const float FLEE_MOVE_SPEED = 5f;
    private int currentIdxPath;
    private List<UnityEngine.Vector3> PathOnVector;
    public GridMap gridMap;
    private Grid_S<PathNode_S> refRootGrid; // grid làm việc trên nó

    // aniamtion
    private BossAnimation bossAnimation;
    private BossAI bossAI;
    private Enemy enemy;

    private Rigidbody2D rb2d;
    private CapsuleCollider2D capsuleCollider2D;

    // public bool hasLeaveGround = false;

    // findPlayer || player vars
    // public FindPlayer_S findPlayer;

    public PlayerMovement playerMovement;
    public int currentVisualDir;
    
    public LayerMask platFormLayerMask;
    public LayerMask wallLayerMask;

    private BossSensor bossSensor;

    private bool IsGroundedVar;

    private const float AIR_MIN_X_VELOCITY = 2.5f;

    private void Start()
    {
        bossAnimation = gameObject.GetComponent<BossAnimation>();
        bossAI = gameObject.GetComponent<BossAI>();
        // findPlayer.setPlayerPathFinding(this);
        // playerAnimation = findPlayer.gameObject.GetComponent<PlayerAnimation>();
        refRootGrid = gridMap.pathFinding.getGrid();

        enemy = gameObject.GetComponent<Enemy>();
        
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        capsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();
        bossSensor = gameObject.GetComponent<BossSensor>();

        // tham chiếu cho level 2
        if(playerMovement == null)
        {
            playerMovement = PlayerManager.Instance.GetPlayerGameObject().GetComponent<PlayerMovement>();
        }
    }

    private void FixedUpdate()
    {
        // Debug.Log("linearVelocityX:" + rb2d.linearVelocityX);
        // IsHole();
        // IsWallInFront();
        if(bossAI.currentEnemyStateAction == BossAI.BossStateAction.Chase || bossAI.currentEnemyStateAction == BossAI.BossStateAction.Patrol)
        {
            MovementPhysicPlatformerHandler();
        }
        // else if(bossAI.currentEnemyStateAction == BossAI.BossStateAction.Jump)
        // {
        //     IsGroundedVar = enemySensor.IsGrounded();
        //     JumpPhysicalPlatformerHandler();
        //     MaintainHorizontalVelocityInAir();
        // }
        else if(bossAI.currentEnemyStateAction == BossAI.BossStateAction.Hurt)
        {
            KnockBackPhysicalPlatformerHandler(currentVisualDir * -1);
        }
        // NEW FOR BOSS @@@
        else if(bossAI.currentEnemyStateAction == BossAI.BossStateAction.Death || 
                bossAI.currentEnemyStateAction == BossAI.BossStateAction.PrepareSkill2)
        {
            StopMovingPhysicalHandler();
        }
        else if(bossAI.currentEnemyStateAction == BossAI.BossStateAction.InvisibleSkill1)
        {
            // cố gắng di chuyển ra xa người chơi và hồi lại máu, có thể đôi khi cũng có thể tấn công
            rb2d.linearVelocity = Vector2.zero;
        }
        else if(bossAI.currentEnemyStateAction == BossAI.BossStateAction.KeeppInvisible)
        {
            MovementPhysicTopDownHandler();
        }
        else if(bossAI.currentEnemyStateAction == BossAI.BossStateAction.Flee)
        {
            FleeMovementPhysicHandler();
        }
        else if(bossAI.currentEnemyStateAction == BossAI.BossStateAction.Recover)
        {
            if (PathOnVector != null) // di chuyển nó có điều kiện cho phép
            {
                MovementPhysicPlatformerHandler();
            }
            else 
            {
                rb2d.linearVelocity = Vector2.zero;
            }
        }
        else if(bossAI.currentEnemyStateAction == BossAI.BossStateAction.Null || 
                bossAI.currentEnemyStateAction == BossAI.BossStateAction.WaitToFight)
        {
            rb2d.linearVelocity = Vector2.zero;
        }
    }

    
    // ========== PHYSICAL ACTION ENEMY HANDLERS ==========
    public void MovementPhysicTopDownHandler()
    {
        if (PathOnVector != null)
        {
            UnityEngine.Vector3 targetPosition = PathOnVector[currentIdxPath];
            
            if (UnityEngine.Vector3.Distance(gameObject.transform.position, targetPosition) <= 0.1f)
            {
                ++currentIdxPath;
                if (currentIdxPath >= PathOnVector.Count)
                {
                    // Debug.Log("Stop Moving");
                    StopMovingPhysicalHandler();
                }
            }
            else
            {
                UnityEngine.Vector3 moveDirection = (targetPosition - gameObject.transform.position).normalized;
                float distanceBefore = UnityEngine.Vector3.Distance(gameObject.transform.position, targetPosition); // for fixing bug
                
                // gameObject.transform.position = gameObject.transform.position + moveDirection * moveSpeed * Time.deltaTime; // C1: di chuyển bằng transform position
                rb2d.linearVelocity = new UnityEngine.Vector2(MOVE_SPEED * moveDirection.x, rb2d.linearVelocityY); // C2: di chuyển bằng rigidbody2D
                LeftORrightTopDown(moveDirection);
            }
        }
    }
     
    private void MovementPhysicPlatformerHandler()
    {
        if (PathOnVector != null)
        {
            UnityEngine.Vector3 targetPosition = PathOnVector[currentIdxPath];
            
            if (UnityEngine.Vector3.Distance(BossPositionHolder.Instance.GetRealBossPosition(), targetPosition) <= 0.6f) // 0.6f == PATROL_REACHED_DISTANCE
            {
                // Debug.Log(Vector3.Distance(realTransform.transform.position, targetPosition));
                // Debug.Log("Đã tới ô thứ: " + currentIdxPath);
                ++currentIdxPath;
                if (currentIdxPath >= PathOnVector.Count)
                {
                    // Debug.Log("Stop Moving");
                    StopMovingPhysicalHandler();
                }
            }
            else
            {
                // Debug.Log(Vector3.Distance(realTransform.transform.position, targetPosition));
                float distanceBefore = UnityEngine.Vector3.Distance(BossPositionHolder.Instance.GetRealBossPosition(), targetPosition); // for fixing bug
                UnityEngine.Vector2 DirToTarget = targetPosition - BossPositionHolder.Instance.GetRealBossPosition();
                
                // VISUAL DIRECTION AND FLIP DIRECTION HANDLER:
                float tmpDirToTargetX = DirToTarget.x;
                if (-0.55f <= tmpDirToTargetX && tmpDirToTargetX <= 0.55f)
                    tmpDirToTargetX = currentVisualDir; // default value if the amount too small
                
                int moveDirX = Math.Sign(tmpDirToTargetX);
                currentVisualDir = LeftOrRightPlatformer(moveDirX) == true ? 1 : -1;
                // Debug.Log("DirToTarget.x: " + DirToTarget.x);
                // Debug.Log(moveDirX);
                
                // PHYSICAL MOVEMENT HANDLER:
                // realTransform.transform.position = realTransform.transform.position + moveDirection * moveSpeed * Time.deltaTime; // C1: di chuyển bằng transform position
                rb2d.linearVelocity = new UnityEngine.Vector2(MOVE_SPEED * moveDirX, rb2d.linearVelocityY); // C2: di chuyển bằng rigidbody2D
                
            }
        }
        else
        {
            // nếu không tìm được đường
            // Debug.Log("Không có đường");
        }
    }
 
    private void JumpPhysicalPlatformerHandler()
    {
        // bool IsGroundedVar = enemySensor.IsGrounded();
        // Debug.Log("IsGroundedVar:" + IsGroundedVar);
        // if(enemyAI.GetIsJumping() == true || IsGroundedVar == false) return;

        if (IsGroundedVar == true && rb2d.linearVelocityY < 0.1f && bossAI.GetIsJumping() == false/*&& hasLeaveGround == false*/) // chưa rời khỏi mặt đất
        {
            const float maxJumpHeight = 3f; // chiều cao tối đa mà Enemy có thể nhảy được
            // Debug.Log(rb2d.linearVelocity.x * PUSH_FORCE + " " +  JUMP_FORCE);
            float obstacleHeight = bossSensor.GetObstacleHeight(); // kiểm tra chiều cao của chướng ngại vật trước khi quyết định lực nhảy 
            // Debug.Log("obstacleHeight:" + obstacleHeight);
            if(obstacleHeight == 0f) return;

            float extraHeight = 0.2f;//0.6f; // cộng dư chiều cao lần 2 :D (cho chắc chắn nhảy phát qua luôn :D)
            float targetHeight = obstacleHeight + extraHeight;
            if(targetHeight > maxJumpHeight) return; // nếu chiều cao vật quá cao thì thôi bỏ không nhảy nữa

            // áp dung công thức vật lý: v = Sqrt(2 × g × h): g: gia tốc kéo vật rơi xuống, v: vận tốc nhảy theo trục y
            float g = Mathf.Abs(Physics2D.gravity.y * rb2d.gravityScale);
            float jumpVelocity = Mathf.Sqrt(2f * g * targetHeight);
            float moveVelocity = currentVisualDir * PUSH_FORCE;
            
            float xLinearVelocity = rb2d.linearVelocityX;
            if(xLinearVelocity == 0)
            {
                moveVelocity = currentVisualDir * PUSH_FORCE;
            }
            else
            {
                moveVelocity = (currentVisualDir * PUSH_FORCE) + xLinearVelocity;
            }
            // Debug.Log("jumpVelocity:" + jumpVelocity + "     obstacleHeight:" + obstacleHeight + "     xVelocity:" + moveVelocity);
            rb2d.linearVelocity = new UnityEngine.Vector2(moveVelocity, jumpVelocity);
            bossAI.SetIsJumpingTrueOutside();
            // hasLeaveGround = true;
            return;
            // isPressedSpace = false;
        }
        else if(IsGroundedVar == true && rb2d.linearVelocityY < 0.1f && bossAI.GetIsJumping() == true) // khi tiếp đất sau khi nhảy thì các điều kiện này sẽ là hợp lý nhất
        {
            bossAI.SetIsJumpingFalseOutside();
        }
    }
    
    private void MaintainHorizontalVelocityInAir()
    {
        if(IsGroundedVar == true) return;
        float velocityX = rb2d.linearVelocityX;
        if(Mathf.Abs(velocityX) <= AIR_MIN_X_VELOCITY)
        {
            rb2d.linearVelocity = new UnityEngine.Vector2(currentVisualDir * AIR_MIN_X_VELOCITY, rb2d.linearVelocityY);
        }
    }

    public void StopMovingByVector()
    {
        PathOnVector = null;
    }
    
    public void StopMovingPhysicalHandler()
    {
        rb2d.linearVelocity = new UnityEngine.Vector2(0, rb2d.linearVelocityY);
        PathOnVector = null;
    }

    public void MoveTo(UnityEngine.Vector3 targetPosition)
    {
        setTargetPosition(targetPosition);
    }

    public void setTargetPosition(UnityEngine.Vector3 targetPosition)
    {
        currentIdxPath = 0;
        UnityEngine.Vector3 enemyPosition = getObjectPosition();

        PathOnVector = PathFinding.Instance.PathOnVector(enemyPosition, targetPosition, out List<PathNode_S> PathOnNode/*, enemy.monsterType*/);   

        // KHÔNG TÌM ĐƯỢC ĐƯỜNG TỚI TARGET
        if (PathOnVector == null || PathOnVector.Count == 0)
        {
            StopMovingPhysicalHandler();
            // Debug.Log("Không có đường");
            return;
        }

        for(int i = 0 ; i < PathOnVector.Count-1 ; i++)
        {
            // Debug.Log("Vẽ từ " + PathOnVector[i] + " Vẽ tới: " + PathOnVector[i]);
            Debug.DrawLine(PathOnVector[i], PathOnVector[i+1], Color.black, 5f);
        }

        if (PathOnVector != null && PathOnVector.Count > 1) // PathOnVector.Count == 1 tức là start = target -> không cần xóa
        {
            PathOnVector.RemoveAt(0);
        }
    }
    
    public void KnockBackPhysicalPlatformerHandler(int AttackerCurrentVisual)
    {
        UnityEngine.Vector3 playerPosition = Player.Instance.GetPlayerPosition();
        UnityEngine.Vector3 dir = (BossPositionHolder.Instance.GetRealBossPosition() - playerPosition).normalized;
        rb2d.linearVelocity = new UnityEngine.Vector3(KNOCK_BACK_HORIZONTAL_FORCE * AttackerCurrentVisual, KNOCK_BACK_VERTICAL_FORCE);
    }
    
    public void FleeMovementPhysicHandler()
    {
        if (PathOnVector != null)
        {
            UnityEngine.Vector3 targetPosition = PathOnVector[currentIdxPath];
            
            if (UnityEngine.Vector3.Distance(BossPositionHolder.Instance.GetRealBossPosition(), targetPosition) <= 0.6f) // 0.6f == PATROL_REACHED_DISTANCE
            {
                // Debug.Log(Vector3.Distance(realTransform.transform.position, targetPosition));
                // Debug.Log("Đã tới ô thứ: " + currentIdxPath);
                ++currentIdxPath;
                if (currentIdxPath >= PathOnVector.Count)
                {
                    // Debug.Log("Stop Moving");
                    StopMovingPhysicalHandler();
                }
                // có nên check nếu có tường ở đây thì đổi hướng không nhỉ ????
            }
            else
            {
                // Debug.Log(Vector3.Distance(realTransform.transform.position, targetPosition));
                float distanceBefore = UnityEngine.Vector3.Distance(BossPositionHolder.Instance.GetRealBossPosition(), targetPosition); // for fixing bug
                UnityEngine.Vector2 DirToTarget = targetPosition - BossPositionHolder.Instance.GetRealBossPosition();
                
                // VISUAL DIRECTION AND FLIP DIRECTION HANDLER:
                float tmpDirToTargetX = DirToTarget.x;
                if (-0.55f <= tmpDirToTargetX && tmpDirToTargetX <= 0.55f)
                    tmpDirToTargetX = currentVisualDir; // default value if the amount too small
                
                int moveDirX = Math.Sign(tmpDirToTargetX);
                // int fleeMoveDirX = -moveDirX;
                currentVisualDir = LeftOrRightPlatformer(moveDirX) == true ? 1 : -1;
                // Debug.Log("DirToTarget.x: " + DirToTarget.x);
                // Debug.Log(moveDirX);
                
                // PHYSICAL MOVEMENT HANDLER:
                // realTransform.transform.position = realTransform.transform.position + moveDirection * moveSpeed * Time.deltaTime; // C1: di chuyển bằng transform position
                rb2d.linearVelocity = new UnityEngine.Vector2(FLEE_MOVE_SPEED * moveDirX, rb2d.linearVelocityY); // C2: di chuyển bằng rigidbody2D
                
            }
        }
        else
        {
            // nếu không tìm được đường
            // Debug.Log("Không có đường");
        }
    }
    
    public void Teleport(Vector3 playerPosition, Vector3 enemyPosition, int playerDirVisual, float minDistBehindPlayer = 1f, float maxDistBehindPlayer = 3f, int maxTry = 8)
    {
        Vector3 telePosition = playerPosition;
        telePosition.y = enemyPosition.y; // giữ lại vị trí y
        for(int i = 0 ; i < maxTry ; i++)
        {
            telePosition.x = playerPosition.x + (UnityEngine.Random.Range(minDistBehindPlayer, maxDistBehindPlayer) * (-1*playerDirVisual));
            // check hợp lệ
            refRootGrid.worldPosToIJPos(telePosition, out int iPos, out int jPos);
            if(refRootGrid.isInGrid(iPos, jPos) == false || 
            gridMap.getIsWalkableByGridPosition(iPos, jPos) == false) continue;
            if(bossSensor.IsBlocked(telePosition)) continue;
            // if (Vector3.Distance(telePosition, playerPosition) < 1f) continue; // không cho nhảy sát người chơi
            gameObject.transform.position = telePosition;
            return;
        }
        gameObject.transform.position = enemyPosition;
        return;
    }
    // ========================================================

    
    // ========== CHECK FUNCTION FOR MOVEMENT PHYSIC ============
    // ========================================================
    

    // ======= SUPPORTING FUNCTION FOR PHYSICAL MOVEMENT =======
    private UnityEngine.Vector3 surroundingPlayerPos(UnityEngine.Vector3 currentPosition) // hiểu hàm này hoạt động
    {
        UnityEngine.Vector3 playerPosition = Player.Instance.GetPlayerPosition();

        int maxAttempts = 10; // số lần thử random
        float minDistance = 6f;
        float maxDistance = 10f;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Random góc và khoảng cách
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float distance = UnityEngine.Random.Range(minDistance, maxDistance);
            UnityEngine.Vector2 offset = new UnityEngine.Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

            // Chuyển worldPos sang gridPos
            UnityEngine.Vector2 candidate = (UnityEngine.Vector2)playerPosition + offset;
            gridMap.pathFinding.getGrid().worldPosToIJPos(candidate, out int i, out int j);

            // Kiểm tra valid positiong && walkable
            if (gridMap.pathFinding.getGrid().isInGrid(i, j) && gridMap.getIsWalkableByGridPosition(i, j))
            {
                return new UnityEngine.Vector3(candidate.x, candidate.y, playerPosition.z); // giữ nguyên trục Z
            }
        }
        return currentPosition;
    }

    public UnityEngine.Vector3 randomPosition(UnityEngine.Vector3 currentPosition)
    {
        int attempt = 10;
        while (attempt > 0)
        {
            float xPos = UnityEngine.Random.Range(-19, 19);
            float yPos = UnityEngine.Random.Range(-9, 9);
            refRootGrid.worldPosToIJPos(new UnityEngine.Vector3(xPos, yPos), out int i, out int j);
            PathNode_S pathNode_S = refRootGrid.getNodeTypeByGridPosition(i, j);
            if (pathNode_S.getIsWalkable() == true)
            {
                return new UnityEngine.Vector3(xPos, yPos);
            }
            else
            {
                continue;
            }
        }
        return currentPosition;
    }

    public UnityEngine.Vector3 getObjectPosition()
    {
        return BossPositionHolder.Instance.GetRealBossPosition();
    }

    public List<UnityEngine.Vector3> getPathOnVector()
    {
        return PathOnVector;
    }
    
    public bool IsHavePath()
    {
        if(PathOnVector == null) return false;
        return true;
    }
    
    public Vector3 FindValidFleeTarget(Vector3 enemyPosition, 
    Vector3 playerPosition, 
    int MAX_DISTANCE = 4, 
    int MIN_DISTANCE = 0,
    int maxTry = 8)
    {
        int miss_time = 0;
        Vector3 dirFlee = -(playerPosition - enemyPosition).normalized;
        Debug.Log("dirFlee:" + dirFlee);
        for(int i = 0 ; i < maxTry ; i++)
        {
            int randomFleeDistance = UnityEngine.Random.Range(MIN_DISTANCE, MAX_DISTANCE + 1-miss_time);
            Vector3 candicatePosition = enemyPosition + dirFlee * randomFleeDistance;
            Debug.Log("candicatePosition:" + candicatePosition);
            // lấy ra vị trí i,j trên grid map
            refRootGrid.worldPosToIJPos(candicatePosition, out int iPos, out int jPos);
            if(refRootGrid.isInGrid(iPos, jPos) == false)
            {
                ++miss_time;
                // StopMovingPhysicalHandler(); // dừng để PathOnVector == null => tìm điểm FleePosition mới
                // Teleport(playerPosition, enemyPosition, playerMovement.GetPlayerVisualDirection());
                continue;
            }
            if(gridMap.getIsWalkableByGridPosition(iPos, jPos) == false)
            {
                Debug.Log("FleeCandicatePosition Eliminated By: getIsWalkableByGridPosition");
                continue;
            }

            // check có xa player hơn không
            float currentDistance = Vector3.Distance(enemyPosition, playerPosition);
            float newDistance = Vector3.Distance(candicatePosition, playerPosition);
            if(newDistance < currentDistance)
            {
                Debug.Log("FleeCandicatePosition Eliminated By: newDistance < currentDistance");
                continue;
            }
            if(bossSensor.IsWallBetween(enemyPosition, candicatePosition) == true)
            {
                Debug.Log("FleeCandicatePosition Eliminated By: IsWallBetween");
                continue;
            }
            if(refRootGrid.IsInsideGridByWorldPosition(candicatePosition) == false)
            {
                Debug.Log("FleeCandicatePosition Eliminated By: IsNullAtPosition");
                continue;
            }

            // return value 
            Debug.Log("Valid Flee Position:" + candicatePosition);
            return candicatePosition;

        }
        Debug.Log("None Flee Position => Return EnemyPosition:" + enemyPosition);
        return enemyPosition;
    }
    // ==========================================================


    // ============== ENEMY VISUAL HANDLERS =====================
    public bool leftORright(UnityEngine.Vector3 moveDirection)
    {
        if (moveDirection.x > 0.01f) // sang phải
        {
            bossAnimation.GetSpriteRenderer().flipX = true;
        }
        else if (moveDirection.x < -0.01f) // sang trái
        {
            bossAnimation.GetSpriteRenderer().flipX = false;
        }
        return bossAnimation.GetSpriteRenderer().flipX;
    }

    public bool LeftORrightTopDown(UnityEngine.Vector3 moveDirection)
    {
        if (moveDirection.x > 0.01f) // sang phải
        {
            bossAnimation.GetSpriteRenderer().flipX = false;
        }
        else if (moveDirection.x < -0.01f) // sang trái
        {
            bossAnimation.GetSpriteRenderer().flipX = true;
        }
        return bossAnimation.GetSpriteRenderer().flipX;
    }

    public bool LeftOrRightPlatformer(int moveDirection)
    {
        if (moveDirection >= 0) // sang phải
        {
            bossAnimation.GetSpriteRenderer().flipX = true;
        }
        else if (moveDirection < 0) // sang trái
        {
            bossAnimation.GetSpriteRenderer().flipX = false;
        }
        return bossAnimation.GetSpriteRenderer().flipX;
    }
    
    private void CurrentVisualDirectionHandler(float moveDirX)
    {
        if(moveDirX == -1) currentVisualDir = -1;
        else currentVisualDir = +1;
    }
    // ===========================================================

    public Grid_S<PathNode_S> getRefRootGrid()
    {
        return refRootGrid;
    }
}
