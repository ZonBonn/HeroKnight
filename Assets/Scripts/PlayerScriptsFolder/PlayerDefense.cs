using UnityEngine;
using System;

public class PlayerDefense : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerHealthStaminaHandler playerHealthStaminaHandler;
    public Action OnBlockIdleIsHited;


    private void Awake()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerHealthStaminaHandler = gameObject.GetComponent<PlayerHealthStaminaHandler>();
    }

    public bool CanBlockByDir(int enemyVisualDir)
    {
        // Debug.Log("playerVisual: " + playerMovement.GetPlayerVisualDirection() + " enemyVisualDir: " + enemyVisualDir);
        if(enemyVisualDir == 0) return false; // nếu hướng là 0 thì vẫn ăn false (không cùng hướng) vì không thể đỡ được dù cùng hay không cùng hướng
        if(playerMovement.GetPlayerVisualDirection() == enemyVisualDir)
        {
            return false; // cùng hướng không đỡ được
        }
            
        return true; // không cùng hướng đỡ được
        // nếu là 0: viên đạn thì không thể đỡ :)
    }

    public bool isBlockingShield()
    {
        return playerMovement.GetPlayerState() == State.BlockIdle;
    }

    // public void ReceiveDamage(float minDamageAttack, float maxDamageAttack, int enemyDir) // xử lý nhận damage player
    // {
    //     bool isBlockingShieldVar = isBlockingShield();
    //     if(isBlockingShieldVar == true && CanBlockByDir(enemyDir) == false)
    //     {
    //         playerHealthStaminaHandler.DamageHealth(UnityEngine.Random.Range(minDamageAttack, maxDamageAttack));
    //     }
    //     else if(isBlockingShieldVar == true && CanBlockByDir(enemyDir) == true)
    //     {
    //         playerHealthStaminaHandler.DamageHealth(0);
    //         OnBlockIdleIsHited?.Invoke();
    //     }
    //     else
    //     {
    //         playerHealthStaminaHandler.DamageHealth(UnityEngine.Random.Range(minDamageAttack, maxDamageAttack));
    //     }
    // }
}
