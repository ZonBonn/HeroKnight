using System.Collections.Generic;
using UnityEngine;

public class movementPathFindingCharacterHandler : MonoBehaviour
{
    public float moveSpeed;
    private int currentIdxPath;
    private List<Vector3> PathOnVector;
    public GridMap gridMap;
    private Grid_S<PathNode_S> refRootGrid; // grid làm việc trên nó

    // aniamtion
    private EnemyAnimation enemyAnimation;
    private Enemy enemy;

    // findPlayer || player vars
    public FindPlayer_S findPlayer;
    private PlayerAnimation playerAnimation;
    private PlayerMovement playerMovement;
    
    private void Start()
    {
        enemyAnimation = gameObject.GetComponent<EnemyAnimation>();
        findPlayer.setPlayerPathFinding(this);
        playerAnimation = findPlayer.gameObject.GetComponent<PlayerAnimation>();
        refRootGrid = gridMap.pathFinding.getGrid();

        enemy = gameObject.GetComponent<Enemy>();
    }

    private void FixedUpdate()
    {
        // if (refEnemyAnimation.getCurrentEnemyState() == EnemyState.Death)
        //     return;
        movementHandler();
    }

    public void movementHandler()
    {
        if (PathOnVector != null)
        {
            Vector3 targetPosition = PathOnVector[currentIdxPath];
            if (Vector3.Distance(gameObject.transform.position, targetPosition) <= Random.Range(0.3f, 0.7f))
            {
                ++currentIdxPath;
                if (currentIdxPath >= PathOnVector.Count)
                {
                    // refEnemyAnimation.getCurrentEnemyState() != EnemyState.Death: không cần thiết vì == Death thì return rồi
                    // if (enemy.monsterType == MonsterType.Ground && refEnemyAnimation.getCurrentEnemyState() != EnemyState.Death)
                    // {
                    //     if(playerAnimation_S.isDied == true)
                    //     {
                    //         refEnemyAnimation.changeEnemyStateAndAnimation(EnemyState.Idle);
                    //     }
                    //     else
                    //     {
                    //         refEnemyAnimation.changeEnemyStateAndAnimation(EnemyState.Attack); // Atack not Idle
                    //     }
                    // }
                    // else if (refEnemyAnimation.monsterType == MonsterType.Sky)
                    // {
                    //     refEnemyAnimation.changeEnemyStateAndAnimation(EnemyState.Fly);
                    // }
                    StopMoving();
                }
            }
            else
            {
                Vector3 moveDirection = (targetPosition - gameObject.transform.position).normalized;
                leftORright(moveDirection);
                float distanceBefore = Vector3.Distance(gameObject.transform.position, targetPosition); // for fixing bug

                gameObject.transform.position = gameObject.transform.position + moveDirection * moveSpeed * Time.deltaTime;
                // if (refEnemyAnimation.monsterType == MonsterType.Ground && refEnemyAnimation.getCurrentEnemyState() != EnemyState.Death)
                // {
                //     refEnemyAnimation.changeEnemyStateAndAnimation(EnemyState.Run);
                // }
                // else if (refEnemyAnimation.monsterType == MonsterType.Sky)
                // {
                //     refEnemyAnimation.changeEnemyStateAndAnimation(EnemyState.Fly);
                // }
            }
        }
    }

    public void StopMoving()
    {
        PathOnVector = null;
    }

    public Vector3 getObjectPosition()
    {
        return gameObject.transform.position;
    }

    public void setTargetPosition(Vector3 targetPosition, out List<PathNode_S> pathOnNode)
    {
        currentIdxPath = 0;
        if (enemy.monsterType == MonsterType.Ground)
        {
            if (playerMovement.GetPlayerState() != State.Die)
            {
                PathOnVector = PathFinding.Instance.PathOnVector(getObjectPosition(), targetPosition, out List<PathNode_S> PathOnNode/*, enemy.monsterType*/);
                pathOnNode = PathOnNode;
            }
            else
            {
                PathOnVector = PathFinding.Instance.PathOnVector(getObjectPosition(), randomPosition(getObjectPosition()), out List<PathNode_S> PathOnNode/*, enemy.monsterType*/);
                pathOnNode = PathOnNode;
            }         
        }
        else // sky monster
        {
            if (playerMovement.GetPlayerState() != State.Die)
            {
                PathOnVector = PathFinding.Instance.PathOnVector(getObjectPosition(), surroundingPlayerPos(getObjectPosition()), out List<PathNode_S> PathOnNode/*, enemy.monsterType*/);
                pathOnNode = PathOnNode;
            }
            else
            {
                PathOnVector = PathFinding.Instance.PathOnVector(getObjectPosition(), randomPosition(getObjectPosition()), out List<PathNode_S> PathOnNode/*, enemy.monsterType*/);
                pathOnNode = PathOnNode;
            }
        }

        if (PathOnVector != null && PathOnVector.Count > 1) // PathOnVector.Count == 1 tức là start = target -> không cần xóa
        {
            PathOnVector.RemoveAt(0);
        }
    }

    private Vector3 surroundingPlayerPos(Vector3 currentPosition) // hiểu hàm này hoạt động
    {
        Vector3 playerPosition = findPlayer.gameObject.transform.position;

        int maxAttempts = 10; // số lần thử random
        float minDistance = 6f;
        float maxDistance = 10f;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Random góc và khoảng cách
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minDistance, maxDistance);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

            // Chuyển worldPos sang gridPos
            Vector2 candidate = (Vector2)playerPosition + offset;
            gridMap.pathFinding.getGrid().worldPosToIJPos(candidate, out int i, out int j);

            // Kiểm tra valid positiong && walkable
            if (gridMap.pathFinding.getGrid().isInGrid(i, j) && gridMap.getIsWalkableByGridPosition(i, j))
            {
                return new Vector3(candidate.x, candidate.y, playerPosition.z); // giữ nguyên trục Z
            }
        }
        return currentPosition;
    }

    public bool leftORright(Vector3 moveDirection)
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

    public List<Vector3> getPathOnVector()
    {
        return PathOnVector;
    }

    public Vector3 randomPosition(Vector3 currentPosition)
    {
        int attempt = 10;
        while (attempt > 0)
        {
            float xPos = Random.Range(-19, 19);
            float yPos = Random.Range(-9, 9);
            refRootGrid.worldPosToIJPos(new Vector3(xPos, yPos), out int i, out int j);
            PathNode_S pathNode_S = refRootGrid.getNodeTypeByGridPosition(i, j);
            if (pathNode_S.getIsWalkable() == true)
            {
                return new Vector3(xPos, yPos);
            }
            else
            {
                continue;
            }
        }
        return currentPosition;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        setTargetPosition(targetPosition, out List<PathNode_S> pathOnNode);
    }
}
