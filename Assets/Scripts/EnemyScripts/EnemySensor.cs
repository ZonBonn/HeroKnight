using UnityEngine;
using System;

public class EnemySensor : MonoBehaviour
{
    private EnemyPathFindingMovement enemyPathFindingMovement;
    private EnemyAI enemyAI;

    void Start()
    {
        enemyPathFindingMovement = gameObject.GetComponent<EnemyPathFindingMovement>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
    }

    
    void Update()
    {
        
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
}
