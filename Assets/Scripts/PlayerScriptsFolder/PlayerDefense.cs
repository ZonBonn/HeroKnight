using UnityEngine;

public class PlayerDefense : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    public bool CanBlock(int enemyVisualDir)
    {
        Debug.Log("playerVisual: " + playerMovement.GetPlayerVisualDirection() + " enemyVisualDir: " + enemyVisualDir);
        if(playerMovement.GetPlayerVisualDirection() == enemyVisualDir)
        {
            return false; // cùng hướng không đỡ được
        }
            
        return true; // không cùng hướng đỡ được
    }
}
