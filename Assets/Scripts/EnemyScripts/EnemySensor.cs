using UnityEngine;
using System;

public class EnemySensor : MonoBehaviour
{
    private EnemyPathFindingMovement enemyPathFindingMovement;
    private EnemyAI enemyAI;
    private CapsuleCollider2D capsuleCollider2D;
    public LayerMask platFormLayerMask;
    public LayerMask wallLayerMask;
    private const float FORWARD_CHECK_EXTRA = 0.7f; // 0.7f;

    void Start()
    {
        enemyPathFindingMovement = gameObject.GetComponent<EnemyPathFindingMovement>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
        capsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();
    }

    public void AlwayTowardToPlayer(){
        Vector3 PlayerPosition = Player.Instance.GetPlayerPosition();
        Vector3 EnemyPosition = gameObject.transform.position;
        Vector2 DirToTarget = PlayerPosition - EnemyPosition;
                
        // handler Visual Direction and Flip Direction
        float tmpDirToTargetX = DirToTarget.x;
        if (-0.55f <= tmpDirToTargetX && tmpDirToTargetX <= 0.55f)
        {
            tmpDirToTargetX = enemyPathFindingMovement.currentVisualDir; // default value if the amount too small
        }
        
        int moveDirX = Math.Sign(tmpDirToTargetX);
        
        enemyPathFindingMovement.currentVisualDir = enemyPathFindingMovement.LeftOrRightPlatformer(moveDirX) == true ? 1 : -1;
    }

    public bool IsSearchedPlayerAround() 
    {
        Vector3 EnemyPosition = gameObject.transform.position;
        Vector2 VisualDir = enemyPathFindingMovement.currentVisualDir == -1 ? Vector2.left : Vector2.right;
        Vector3 maxDistanceVisualPoint = VisualDir == Vector2.left ? enemyAI.chaseLeftPoint.position : enemyAI.chaseRightPoint.position;
        // đây sẽ là hai đểm ChaseWaypointA hoặc ChaseWaypointB
        float DistanceVisual = Vector3.Distance(EnemyPosition, maxDistanceVisualPoint);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(EnemyPosition, VisualDir, DistanceVisual, enemyAI.playerLayerMask);
        Debug.DrawLine(EnemyPosition, maxDistanceVisualPoint, Color.darkBlue, 0.1f);
        if (raycastHit2D.collider != null)
        {
            return true;
        }
        return false;
    }

    public float GetObstacleHeight() // viết lại cái hàm này 
    {
        Vector3 dir = enemyPathFindingMovement.currentVisualDir == -1 ? Vector3.left : Vector3.right;
        Bounds b = capsuleCollider2D.bounds;
        Vector3 basePos = new Vector3(b.center.x, b.min.y, 0f);
        float[] heights = {0.1f, 0.5f, 1.0f, 1.5f, 2f, 2.5f};
        for(int idx = heights.Length-2 ; idx >= 0; idx--)
        {
            Vector3 origin = basePos + Vector3.up * heights[idx];
            float RayLenght = (capsuleCollider2D.size.x * .5f) + FORWARD_CHECK_EXTRA;
            RaycastHit2D hitplatFormLayerMask = Physics2D.Raycast(origin, dir, RayLenght, platFormLayerMask);
            RaycastHit2D hitWallLayerMask = Physics2D.Raycast(origin, dir, RayLenght, wallLayerMask);
            Debug.DrawRay(origin, dir*RayLenght, Color.yellow);
            if (hitplatFormLayerMask.collider != null || hitWallLayerMask.collider != null)
            {
                return heights[idx+1]; // dư chiều cao ra chút cho chắc chắn nhảy qua
            }
                
        }
        return 0f; // không có vật cản
    }

    public bool IfCanJumpOverTheInFrontWall()
    {
        // Debug.Log("IsWallOrGroundInFront:" + IsWallOrGroundInFront() + "  IsWallOrGroundTooHigh:" + !IsWallOrGroundTooHigh());
        return IsWallOrGroundInFront() && !IsWallOrGroundTooHigh();
    }

    private bool IsWallOrGroundInFront()
    {
        UnityEngine.Vector3 dir;
        dir = enemyPathFindingMovement.currentVisualDir == -1 ? UnityEngine.Vector3.left : UnityEngine.Vector3.right;
        UnityEngine.Vector3 origin = capsuleCollider2D.bounds.center;
        float RayLenght = (capsuleCollider2D.size.x * .5f) + FORWARD_CHECK_EXTRA;

        RaycastHit2D rayCastHit2DWallLayerMask = Physics2D.Raycast(origin, dir, RayLenght, wallLayerMask);
        RaycastHit2D rayCastHit2DPlatformLayerMask = Physics2D.Raycast(origin, dir, RayLenght, platFormLayerMask);
        Debug.DrawRay(origin, dir * RayLenght, Color.blueViolet);
        if(rayCastHit2DWallLayerMask.collider != null || rayCastHit2DPlatformLayerMask.collider != null) // có va chạm với wallLayerMask -> có tường -> true
        {
            return true; // lẽ ra chỗ này return true nhưng mà có vẻ cơ chế nhảy không cần thiết lắm
        }
        return false;
    }

    private bool IsBlockedDuringJump() // hàm này nếu đúng thì có nghĩa là cao hơn enemy => bỏ không nhảy được còn đâu thì ngược lại
    {
        // mô phỏng một capsule tại đây nhảy xem có nhảy qua được không ?
        // UnityEngine.Vector3 direct = new UnityEngine.Vector3(currentVisualDir, 1).normalized;
        UnityEngine.Vector3 direct = UnityEngine.Vector3.up;

        RaycastHit2D rayCastHit2D = Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, 
        capsuleCollider2D.bounds.size, 
        capsuleCollider2D.direction, 
        0f, 
        direct, 
        3.5f,  // độ cao cao nhất mà có thể nhảy được tính từ 0
        wallLayerMask);

        if(rayCastHit2D.collider != null)
        {
            return false;
        }
        return true;
    }

    private bool IsWallOrGroundTooHigh()
    {
        const float maxJumpHeight = 2.5f;
        UnityEngine.Vector3 StartPoint = new UnityEngine.Vector3(capsuleCollider2D.bounds.center.x, capsuleCollider2D.bounds.center.y + maxJumpHeight);
        UnityEngine.Vector3 directScanning = enemyPathFindingMovement.currentVisualDir == -1 ? UnityEngine.Vector3.left : UnityEngine.Vector3.right;
        float distanceScanning = capsuleCollider2D.size.x + 0.2f; 

        RaycastHit2D rayCastHit2DWallLayerMask = Physics2D.CapsuleCast(
        StartPoint, 
        capsuleCollider2D.size, 
        capsuleCollider2D.direction, 
        0f, 
        directScanning, 
        distanceScanning, 
        wallLayerMask
        );
        RaycastHit2D rayCastHit2DPlatformLayerMask = Physics2D.CapsuleCast(
        StartPoint, 
        capsuleCollider2D.size, 
        capsuleCollider2D.direction, 
        0f, 
        directScanning, 
        distanceScanning, 
        platFormLayerMask
        );

        if(rayCastHit2DWallLayerMask.collider != null || rayCastHit2DPlatformLayerMask.collider != null) // kiểm tra tường phía trên -> nếu có va chạm với tường -> cao -> true
        {
            return true; // lẽ ra chỗ này return true nhưng mà có vẻ cơ chế nhảy không cần thiết lắm
        }
        return false;
    }

    public bool IsGrounded()
    {
        // RaycastHit2D rayCastHit2D = Physics2D.BoxCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, 0f, UnityEngine.Vector2.down, 0.01f, platFormLayerMask);
        RaycastHit2D rayCastHit2D = Physics2D.Raycast(capsuleCollider2D.bounds.center, UnityEngine.Vector3.down, capsuleCollider2D.size.y * 0.5f + 0.05f, platFormLayerMask);
        
        Debug.DrawRay(capsuleCollider2D.bounds.center, UnityEngine.Vector3.down * (capsuleCollider2D.size.y * 0.5f + 0.05f), Color.green); // in xem box cast có dai qua k
        if (rayCastHit2D.collider != null) // nếu có va chạm với platformLayerMask -> có đang chạm mặt đất -> true
        {
            return true;
        }
        return false;
    }
    
    public bool IsHole()
    {
        var b = capsuleCollider2D.bounds;
        UnityEngine.Vector3 DownLeft = b.min;
        UnityEngine.Vector3 DownRight = new UnityEngine.Vector3(b.max.x, b.min.y);

        UnityEngine.Vector3 dir;
        dir = enemyPathFindingMovement.currentVisualDir == -1 ? DownLeft : DownRight;
        UnityEngine.Vector3 StartPoint =  dir + (enemyPathFindingMovement.currentVisualDir == -1 ? UnityEngine.Vector3.left : UnityEngine.Vector3.right) * 0.1f;
        RaycastHit2D rayCastHit2D = Physics2D.Raycast(StartPoint, UnityEngine.Vector2.down, 1.2f, platFormLayerMask);

        Debug.DrawRay(StartPoint, UnityEngine.Vector2.down * 1.2f, Color.pink);
        float realDistance = rayCastHit2D.distance;
        
        if (rayCastHit2D.collider != null) // nếu có va chạm với collider -> platFormLayerMask -> không có hố -> false
        {
            return false; 
        }
        return true; // lẽ ra chỗ này return true nhưng mà có vẻ cơ chế nhảy không cần thiết lắm
    }
}
