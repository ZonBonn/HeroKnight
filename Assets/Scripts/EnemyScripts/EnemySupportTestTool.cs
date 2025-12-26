using UnityEngine;

public class EnemySupportTestTool : MonoBehaviour
{
    private HealthHandler enemyHealthHandler;
    private EnemyAI enemyAI;
    private HealthBar enemyHealthBar;



    void Start()
    {
        enemyHealthHandler = gameObject.GetComponent<HealthHandler>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
        enemyHealthBar = enemyHealthHandler.GetHealthBar();
    }

    
    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.L))
        {
            if(enemyAI.GetIsDied() == true)
            {
                enemyHealthHandler.Heal(100);
                //  enemyPathFindingMovement.StopMovingPhysicalHandler();
                enemyAI.currentEnemyStateAction = EnemyAI.EnemyStateAction.Recovery;
                gameObject.layer = LayerMask.NameToLayer("Enemy");
                enemyHealthBar.gameObject.SetActive(true);
                enemyAI.SetIsDied(false); // danger thay đổi biến như này thì nguy hiểm nhưng trong test tool thì vẫn được chấp nhận
            }
        }
        #endif
    }

    
}
