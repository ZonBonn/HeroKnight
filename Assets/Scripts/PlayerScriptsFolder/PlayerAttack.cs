using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerAnimation playerAnimation;
    private PlayerHealthStaminaHandler playerHealthStaminaHandler;
    private PlayerStaminaSystem playerStaminaSystem;

    public LayerMask enemyLayerMask;

    private const float normalAttackManaCost = 25f;

    private CapsuleCollider2D capsuleCollider2D;

    private bool canEnoughManaToAttack;

    private void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerAnimation = gameObject.GetComponent<PlayerAnimation>();
        playerHealthStaminaHandler = gameObject.GetComponent<PlayerHealthStaminaHandler>();
        capsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();

        playerStaminaSystem = playerHealthStaminaHandler.GetPlayerStaminaSystem();
    }

    public void CreatePointAttack(Sprite[] currentSprite)
    {
        canEnoughManaToAttack = playerHealthStaminaHandler.TryToUseStamina(normalAttackManaCost);
        if(canEnoughManaToAttack == true)
        {
            Vector3 playerPosition = Player.Instance.GetPlayerPosition();
            // int visualDir = playerMovement.FlipDir();
            int visualDir = playerMovement.GetPlayerVisualDirection();
            const float attackDistance = 0.7f;
            // Debug.Log("playerPosition.x:" + playerPosition.x + " visualDir:" + visualDir + " attackDistance:" + attackDistance);
            Vector3 attackPoint = new Vector3(playerPosition.x + (visualDir * attackDistance), playerPosition.y);
            // Debug.Log(attackPoint);
            RaycastHit2D rayCastHit2D = IsHitedEnemy(currentSprite, attackDistance);
            if (rayCastHit2D.collider != null)
            {
                DamageEnemy(rayCastHit2D, currentSprite);
            }
        }
        
    }

    public RaycastHit2D IsHitedEnemy(Sprite[] currentSprite, float attackDistance) // return hited or not hited và xử lý thêm phần hp nữa
    {
        Vector3 DirRaycast = playerMovement.GetPlayerVisualDirection() == 1 ? Vector3.right : Vector3.left;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(capsuleCollider2D.bounds.center, DirRaycast, attackDistance, enemyLayerMask);
        return raycastHit2D;
    }

    public void DamageEnemy(RaycastHit2D raycastHit2D, Sprite[] currentSprite)
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
    }

    public float GetNormalAttackManaCost()
    {
        return normalAttackManaCost;
    }

    public bool GetCanEnoughManaToAttack()
    {
        return canEnoughManaToAttack;
    }

    public bool CanEnoughManaForNormalAttack()
    {
        return playerStaminaSystem.GetCurrentStamina() >= normalAttackManaCost;
    }
}
