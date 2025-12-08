using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerAnimation playerAnimation;

    public LayerMask enemyLayerMask;

    private CapsuleCollider2D capsuleCollider2D;

    private void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerAnimation = gameObject.GetComponent<PlayerAnimation>();
        capsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();
    }

    public void CreatePointAttack(Sprite[] currentSprite)
    {
        Vector3 playerPosition = Player.Instance.GetPlayerPosition();
        // int visualDir = playerMovement.FlipDir();
        int visualDir = playerMovement.GetPlayerVisualDirection();
        const float attackDistance = 0.7f;
        // Debug.Log("playerPosition.x:" + playerPosition.x + " visualDir:" + visualDir + " attackDistance:" + attackDistance);
        Vector3 attackPoint = new Vector3(playerPosition.x + (visualDir * attackDistance), playerPosition.y);
        Debug.Log(attackPoint);
        if (IsHitedEnemy(attackPoint, currentSprite, attackDistance))
        {
            
        }
    }

    public bool IsHitedEnemy(Vector3 attackPoint, Sprite[] currentSprite, float attackDistance) // return hited or not hited và xử lý thêm phần hp nữa
    {
        Vector3 DirRaycast = playerMovement.GetPlayerVisualDirection() == 1 ? Vector3.right : Vector3.left;
        float DistanceRaycast = Vector3.Distance(Player.Instance.GetPlayerPosition(), attackPoint);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(capsuleCollider2D.bounds.center, DirRaycast, attackDistance, enemyLayerMask);
        if(raycastHit2D.collider != null) // hited
        {
            // damage enemy handler
            GameObject enemyGameObject = raycastHit2D.collider.gameObject;
            HealthHandler enemyHealthHandler = enemyGameObject.GetComponent<HealthHandler>();
            if(currentSprite == playerAnimation.Attack1Sprites || currentSprite == playerAnimation.Attack2Sprites)
            {
                enemyHealthHandler.Damage(UnityEngine.Random.Range(20, 25));
            }
            else // currentSprite == playerAnimation.Attack3Sprites
            {
                enemyHealthHandler.Damage(UnityEngine.Random.Range(30, 35));
            }
            return true;
        }
        return false;
    }
}
