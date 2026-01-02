using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;

public class EnemyPathFindingMovement : MonoBehaviour
{
    public const float MOVE_SPEED = 2f;
    public const float JUMP_FORCE = 14f;
    private const float PUSH_FORCE = 5.5f;
    private const float KNOCK_BACK_HORIZONTAL_FORCE = 3f;
    private const float KNOCK_BACK_VERTICAL_FORCE = 1.5f;
    private int currentIdxPath;
    private List<UnityEngine.Vector3> PathOnVector;
    public GridMap gridMap;
    private Grid_S<PathNode_S> refRootGrid; // grid làm việc trên nó

    // aniamtion
    private EnemyAnimation enemyAnimation;
    private EnemyAI enemyAI;
    private Enemy enemy;

    private Rigidbody2D rb2d;
    private CapsuleCollider2D capsuleCollider2D;

    // findPlayer || player vars
    // public FindPlayer_S findPlayer;

    public PlayerMovement playerMovement;
    public int currentVisualDir;
    
    public LayerMask platFormLayerMask;
    public LayerMask wallLayerMask;

    private EnemySensor enemySensor;

    private void Start()
    {
        enemyAnimation = gameObject.GetComponent<EnemyAnimation>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
        // findPlayer.setPlayerPathFinding(this);
        // playerAnimation = findPlayer.gameObject.GetComponent<PlayerAnimation>();
        refRootGrid = gridMap.pathFinding.getGrid();

        enemy = gameObject.GetComponent<Enemy>();
        
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        capsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();
        enemySensor = gameObject.GetComponent<EnemySensor>();

        // tham chiếu cho level 2
        if(playerMovement == null)
        {
            playerMovement = PlayerManager.Instance.GetPlayerGameObject().GetComponent<PlayerMovement>();
        }
    }

    private void FixedUpdate()
    {
        // IsHole();
        // IsWallInFront();
        if(enemyAI.currentEnemyStateAction == EnemyAI.EnemyStateAction.Chase || enemyAI.currentEnemyStateAction == EnemyAI.EnemyStateAction.Patrol)
        {
            MovementPhysicPlatformerHandler();
        }
        else if(enemyAI.currentEnemyStateAction == EnemyAI.EnemyStateAction.Jump)
        {
            JumpPhysicalPlatformerHandler();
        }
        else if(enemyAI.currentEnemyStateAction == EnemyAI.EnemyStateAction.Hurt)
        {
            KnockBackPhysicalPlatformerHandler(currentVisualDir * -1);
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
                    Debug.Log("Stop Moving");
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
    
    public void MovementPhysicPlatformerHandler()
    {
        if (PathOnVector != null)
        {
            UnityEngine.Vector3 targetPosition = PathOnVector[currentIdxPath];
            
            if (UnityEngine.Vector3.Distance(gameObject.transform.position, targetPosition) <= 0.6f)
            {
                // Debug.Log(Vector3.Distance(gameObject.transform.position, targetPosition));
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
                // Debug.Log(Vector3.Distance(gameObject.transform.position, targetPosition));
                float distanceBefore = UnityEngine.Vector3.Distance(gameObject.transform.position, targetPosition); // for fixing bug
                UnityEngine.Vector2 DirToTarget = targetPosition - gameObject.transform.position;
                
                // VISUAL DIRECTION AND FLIP DIRECTION HANDLER:
                float tmpDirToTargetX = DirToTarget.x;
                if (-0.55f <= tmpDirToTargetX && tmpDirToTargetX <= 0.55f)
                    tmpDirToTargetX = currentVisualDir; // default value if the amount too small
                
                int moveDirX = Math.Sign(tmpDirToTargetX);
                currentVisualDir = LeftOrRightPlatformer(moveDirX) == true ? 1 : -1;
                // Debug.Log("DirToTarget.x: " + DirToTarget.x);
                // Debug.Log(moveDirX);
                
                // PHYSICAL MOVEMENT HANDLER:
                // gameObject.transform.position = gameObject.transform.position + moveDirection * moveSpeed * Time.deltaTime; // C1: di chuyển bằng transform position
                rb2d.linearVelocity = new UnityEngine.Vector2(MOVE_SPEED * moveDirX, rb2d.linearVelocityY); // C2: di chuyển bằng rigidbody2D
                
            }
        }
    }

    public void JumpPhysicalPlatformerHandler()
    {
        if (enemySensor.IsGrounded() == true)
        {
            // Debug.Log(rb2d.linearVelocity.x * PUSH_FORCE + " " +  JUMP_FORCE);
            rb2d.linearVelocity = new UnityEngine.Vector2(currentVisualDir * PUSH_FORCE, JUMP_FORCE);
            // isPressedSpace = false;
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
        if (enemy.monsterType == MonsterType.Ground)
        {
            // if (playerMovement.GetPlayerState() != State.Die)
            // {
            //     PathOnVector = PathFinding.Instance.PathOnVector(enemyPosition, targetPosition, out List<PathNode_S> PathOnNode, enemy.monsterType);
            // }
            // else
            // {
            //     PathOnVector = PathFinding.Instance.PathOnVector(enemyPosition, randomPosition(enemyPosition), out List<PathNode_S> PathOnNode, enemy.monsterType);
            // }      
            PathOnVector = PathFinding.Instance.PathOnVector(enemyPosition, targetPosition, out List<PathNode_S> PathOnNode/*, enemy.monsterType*/);   
        }
        else // sky monster
        {
            // if (playerMovement.GetPlayerState() != State.Die)
            // {
            //     PathOnVector = PathFinding.Instance.PathOnVector(enemyPosition, surroundingPlayerPos(enemyPosition), out List<PathNode_S> PathOnNode, enemy.monsterType);
            // }
            // else
            // {
            //     PathOnVector = PathFinding.Instance.PathOnVector(enemyPosition, randomPosition(enemyPosition), out List<PathNode_S> PathOnNode, enemy.monsterType);
            // }
            PathOnVector = PathFinding.Instance.PathOnVector(enemyPosition, surroundingPlayerPos(enemyPosition), out List<PathNode_S> PathOnNode/*, enemy.monsterType*/);
        }

        if (PathOnVector != null && PathOnVector.Count > 1) // PathOnVector.Count == 1 tức là start = target -> không cần xóa
        {
            PathOnVector.RemoveAt(0);
        }
    }
    
    public void KnockBackPhysicalPlatformerHandler(int AttackerCurrentVisual)
    {
        UnityEngine.Vector3 playerPosition = Player.Instance.GetPlayerPosition();
        UnityEngine.Vector3 dir = (gameObject.transform.position - playerPosition).normalized;
        rb2d.linearVelocity = new UnityEngine.Vector3(KNOCK_BACK_HORIZONTAL_FORCE * AttackerCurrentVisual, KNOCK_BACK_VERTICAL_FORCE);
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
        return gameObject.transform.position;
    }

    public List<UnityEngine.Vector3> getPathOnVector()
    {
        return PathOnVector;
    }
    // ==========================================================


    // ============== ENEMY VISUAL HANDLERS =====================
    public bool leftORright(UnityEngine.Vector3 moveDirection)
    {
        if (moveDirection.x > 0.01f) // sang phải
        {
            enemyAnimation.GetSpriteRenderer().flipX = true;
        }
        else if (moveDirection.x < -0.01f) // sang trái
        {
            enemyAnimation.GetSpriteRenderer().flipX = false;
        }
        return enemyAnimation.GetSpriteRenderer().flipX;
    }

    public bool LeftORrightTopDown(UnityEngine.Vector3 moveDirection)
    {
        if (moveDirection.x > 0.01f) // sang phải
        {
            enemyAnimation.GetSpriteRenderer().flipX = false;
        }
        else if (moveDirection.x < -0.01f) // sang trái
        {
            enemyAnimation.GetSpriteRenderer().flipX = true;
        }
        return enemyAnimation.GetSpriteRenderer().flipX;
    }

    public bool LeftOrRightPlatformer(int moveDirection)
    {
        if (moveDirection >= 0) // sang phải
        {
            enemyAnimation.GetSpriteRenderer().flipX = true;
        }
        else if (moveDirection < 0) // sang trái
        {
            enemyAnimation.GetSpriteRenderer().flipX = false;
        }
        return enemyAnimation.GetSpriteRenderer().flipX;
    }
    
    private void CurrentVisualDirectionHandler(float moveDirX)
    {
        if(moveDirX == -1) currentVisualDir = -1;
        else currentVisualDir = +1;
    }
    // ===========================================================
}

// REFACTED CODE
// public bool IsGrounded()
    // {
    //     // RaycastHit2D rayCastHit2D = Physics2D.BoxCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, 0f, UnityEngine.Vector2.down, 0.01f, platFormLayerMask);
    //     RaycastHit2D rayCastHit2D = Physics2D.Raycast(capsuleCollider2D.bounds.center, UnityEngine.Vector3.down, capsuleCollider2D.size.y * 0.5f + 0.05f, platFormLayerMask);
        
    //     // Debug.DrawRay(capsuleCollider2D.bounds.center, ) // in xem box cast có dai qua k
    //     if (rayCastHit2D.collider != null) // nếu có va chạm với platformLayerMask -> có đang chạm mặt đất -> true
    //     {
    //         return true;
    //     }
    //     return false;
    // }
    
    // public bool IsHole()
    // {
    //     var b = capsuleCollider2D.bounds;
    //     UnityEngine.Vector3 DownLeft = b.min;
    //     UnityEngine.Vector3 DownRight = new UnityEngine.Vector3(b.max.x, b.min.y);

    //     UnityEngine.Vector3 dir;
    //     dir = currentVisualDir == -1 ? DownLeft : DownRight;
    //     UnityEngine.Vector3 StartPoint =  dir + (currentVisualDir == -1 ? UnityEngine.Vector3.left : UnityEngine.Vector3.right) * 0.1f;
    //     RaycastHit2D rayCastHit2D = Physics2D.Raycast(StartPoint, UnityEngine.Vector2.down, 1.2f, platFormLayerMask);

    //     // Debug.DrawRay(StartPoint, UnityEngine.Vector2.down * 1.2f, Color.pink);
    //     float realDistance = rayCastHit2D.distance;
        
    //     if (rayCastHit2D.collider != null) // nếu có va chạm với collider -> platFormLayerMask -> không có hố -> false
    //     {
    //         return false; 
    //     }
    //     return true; // lẽ ra chỗ này return true nhưng mà có vẻ cơ chế nhảy không cần thiết lắm
    // }

    // public bool IfCanJumpOverTheInFrontWall()
    // {
    //     Debug.Log("IsWallInFront:" + IsWallOrGroundInFront() + " IsWallOrGroundTooHigh:" + !IsWallOrGroundTooHigh());
    //     return IsWallOrGroundInFront() && !IsWallOrGroundTooHigh();
    // }

    // private bool IsWallOrGroundInFront()
    // {
    //     UnityEngine.Vector3 dir;
    //     dir = currentVisualDir == -1 ? UnityEngine.Vector3.left : UnityEngine.Vector3.right;
    //     UnityEngine.Vector3 origin = capsuleCollider2D.bounds.center;
    //     float RayLenght = (capsuleCollider2D.size.x * .5f) + 0.5f;

    //     RaycastHit2D rayCastHit2DWallLayerMask = Physics2D.Raycast(origin, dir, RayLenght, wallLayerMask);
    //     RaycastHit2D rayCastHit2DPlatformLayerMask = Physics2D.Raycast(origin, dir, RayLenght, platFormLayerMask);
    //     // Debug.DrawRay(origin, dir * RayLenght, Color.blueViolet);
    //     if(rayCastHit2DWallLayerMask.collider != null || rayCastHit2DPlatformLayerMask.collider != null) // có va chạm với wallLayerMask -> có tường -> true
    //     {
    //         return true; // lẽ ra chỗ này return true nhưng mà có vẻ cơ chế nhảy không cần thiết lắm
    //     }
    //     return false;
    // }

    // private bool IsBlockedDuringJump() // hàm này nếu đúng thì có nghĩa là cao hơn enemy => bỏ không nhảy được còn đâu thì ngược lại
    // {
    //     // mô phỏng một capsule tại đây nhảy xem có nhảy qua được không ?
    //     // UnityEngine.Vector3 direct = new UnityEngine.Vector3(currentVisualDir, 1).normalized;
    //     UnityEngine.Vector3 direct = UnityEngine.Vector3.up;

    //     RaycastHit2D rayCastHit2D = Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, 
    //     capsuleCollider2D.bounds.size, 
    //     capsuleCollider2D.direction, 
    //     0f, 
    //     direct, 
    //     3.5f,  // độ cao cao nhất mà có thể nhảy được tính từ 0
    //     wallLayerMask);

    //     if(rayCastHit2D.collider != null)
    //     {
    //         return false;
    //     }
    //     return true;
    // }

    // private bool IsWallOrGroundTooHigh()
    // {
    //     const float maxJumpHeight = 2.5f;
    //     UnityEngine.Vector3 StartPoint = new UnityEngine.Vector3(capsuleCollider2D.bounds.center.x, capsuleCollider2D.bounds.center.y + maxJumpHeight);
    //     UnityEngine.Vector3 directScanning = currentVisualDir == -1 ? UnityEngine.Vector3.left : UnityEngine.Vector3.right;
    //     float distanceScanning = capsuleCollider2D.size.x + 0.2f; 

    //     RaycastHit2D rayCastHit2DWallLayerMask = Physics2D.CapsuleCast(
    //     StartPoint, 
    //     capsuleCollider2D.size, 
    //     capsuleCollider2D.direction, 
    //     0f, 
    //     directScanning, 
    //     distanceScanning, 
    //     wallLayerMask
    //     );
    //     RaycastHit2D rayCastHit2DPlatformLayerMask = Physics2D.CapsuleCast(
    //     StartPoint, 
    //     capsuleCollider2D.size, 
    //     capsuleCollider2D.direction, 
    //     0f, 
    //     directScanning, 
    //     distanceScanning, 
    //     platFormLayerMask
    //     );

    //     if(rayCastHit2DWallLayerMask.collider != null || rayCastHit2DPlatformLayerMask.collider != null) // kiểm tra tường phía trên -> nếu có va chạm với tường -> cao -> true
    //     {
    //         return true; // lẽ ra chỗ này return true nhưng mà có vẻ cơ chế nhảy không cần thiết lắm
    //     }
    //     return false;
    // }