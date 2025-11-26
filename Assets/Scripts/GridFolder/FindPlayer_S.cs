using UnityEngine;
using System.Collections.Generic;

public class FindPlayer_S : MonoBehaviour
{

    // Enemy Find Path
    public List<movementPathFindingCharacterHandler> enemyFindPathToPlayer;
    [SerializeField] float delayFindPathForEach;
    private float timer;

    private PlayerAnimation playerAnimation;

    void Start()
    {
        timer = 0f;
        playerAnimation = gameObject.GetComponent<PlayerAnimation>();
        // playerAnimation.OnTriggerAfterPlayerDeath += changeDelayFindPathForEachAfterPlayerDeath;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            for (int idx = enemyFindPathToPlayer.Count - 1; idx >= 0; idx--)
            {
                if (enemyFindPathToPlayer[idx] != null)
                {
                    enemyFindPathToPlayer[idx].setTargetPosition(getPlayerPosition(), out List<PathNode_S> pathOnNode);
                }
                else
                {
                    enemyFindPathToPlayer[idx] = enemyFindPathToPlayer[enemyFindPathToPlayer.Count - 1];
                    enemyFindPathToPlayer.RemoveAt(enemyFindPathToPlayer.Count - 1);
                }
            }
            timer = delayFindPathForEach;
        }
    }

    public void setPlayerPathFinding(movementPathFindingCharacterHandler enemyPathFinding)
    {
        enemyFindPathToPlayer.Add(enemyPathFinding);
    }

    public Vector3 getPlayerPosition()
    {
        return gameObject.transform.position;
    }

    private void changeDelayFindPathForEachAfterPlayerDeath()
    {
        delayFindPathForEach = Random.Range(5, 7);
    }
}
