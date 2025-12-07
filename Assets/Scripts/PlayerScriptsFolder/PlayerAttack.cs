using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    public void CreatePointAttack()
    {
        Vector3 playerPosition = Player.Instance.GetPlayerPosition();
        // int visualDir = playerMovement.FlipDir();
        int visualDir = playerMovement.GetPlayerVisualDirection();
        const float attackDistance = 0.7f;

        Vector3 attackPoint = new Vector3(playerPosition.x * visualDir + attackDistance, playerPosition.y);
        Debug.Log(attackPoint);
    }
}
