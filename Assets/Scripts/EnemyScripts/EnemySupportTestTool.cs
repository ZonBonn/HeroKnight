using UnityEngine;

public class EnemySupportTestTool : MonoBehaviour
{
    private IEnemyAI enemyAI;

    void Start()
    {
        enemyAI = gameObject.GetComponent<IEnemyAI>();
    }

    
    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.L) && enemyAI != null)
            {
                if(enemyAI.GetIsDied() == true)
                {
                    enemyAI.Revive(); // tự gọi cho nó tự thực hiện Revive() của từng đối tượng (Decoupling thấp)
                }
            }
        #endif
    }

    
}
