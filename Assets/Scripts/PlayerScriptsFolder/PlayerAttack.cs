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
            const float attackDistance = 0.85f;
            // Debug.Log("playerPosition.x:" + playerPosition.x + " visualDir:" + visualDir + " attackDistance:" + attackDistance);
            Vector3 attackPoint = new Vector3(playerPosition.x + (visualDir * attackDistance), playerPosition.y);
            // Debug.Log(attackPoint);
            RaycastHit2D rayCastHit2D = IsHitedEnemy(currentSprite, attackDistance);
            if (rayCastHit2D.collider != null)
            {
                // if(rayCastHit2D.collider.gameObject.CompareTag("BossDeathBringer") == true)
                // {
                //     BossLevelCombatManager bossLevelCombatManager = rayCastHit2D.collider.gameObject.GetComponent<BossLevelCombatManager>();
                //     if(bossLevelCombatManager == null) Debug.Log("bossLevelCombatManager == null");
                //     if(bossLevelCombatManager.getCanMakeDamage() == true)
                //     {
                //         DamageEnemy(rayCastHit2D, currentSprite);
                //     }
                // }
                // else
                // {
                //     DamageEnemy(rayCastHit2D, currentSprite);
                // }
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
        // HealthHandler enemyHealthHandler = enemyGameObject.GetComponent<HealthHandler>();
        Enemy enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.getFeature(out float minDamageReceived, out float maxDamageReceived, out float minDamageAttack, out float maxDamageAttack);

        // xứ lý riêng boss => sẽ không clean nhưng thôi tạm thời thế đã
        // bool isBossDeathBringer = enemyGameObject.CompareTag("BossDeathBringer");
        // BossLevelCombatManager bossLevelCombatManager  = enemyGameObject.GetComponent<BossLevelCombatManager>();
        // if(bossLevelCombatManager == null) Debug.Log("bossLevelCombatManager == null");
        // BossHealthHandler bossHealthHandler = null;
        
        
        // if(isBossDeathBringer == true)
        // {
        //     bossHealthHandler = enemyGameObject.GetComponent<BossHealthHandler>();
        //     damageable = enemyGameObject.GetComponent<IDamageable>();
        // }
        IDamageable damageable = enemyGameObject.GetComponent<IDamageable>();
        

        if(currentSprite == playerAnimation.Attack1Sprites)
        {
            // if(isBossDeathBringer == true && bossLevelCombatManager != null)
            // {
            //     // if(bossLevelCombatManager.getCanMakeDamage() == true)
            //     // {
            //     //     bossHealthHandler.DamageBoss(minDamageReceived);
            //     // }
            //     // else
            //     // {
            //     //     bossHealthHandler.DamageBoss(0);
            //     // }
            //     damageable.Damage(minDamageReceived);
            // }
            // else
            // {
            //     enemyHealthHandler.Damage(minDamageReceived);
            // }
                
            damageable.Damage(minDamageReceived);

        }
        else if (currentSprite == playerAnimation.Attack2Sprites)
        {
            // if(isBossDeathBringer == true && bossLevelCombatManager != null)
            // {
            //     // if(bossLevelCombatManager.getCanMakeDamage() == true)
            //     // {
            //     //     bossHealthHandler.DamageBoss(maxDamageReceived);
            //     // }
            //     // else
            //     // {
            //     //     bossHealthHandler.DamageBoss(0);
            //     // }
            //     damageable.Damage(maxDamageReceived);
            // }
            // else
            //     enemyHealthHandler.Damage(maxDamageReceived);

            damageable.Damage(maxDamageReceived);
        }
        else // currentSprite == playerAnimation.Attack3Sprites
        {
            // if(isBossDeathBringer == true && bossLevelCombatManager != null)
            // {
            //     // if(bossLevelCombatManager.getCanMakeDamage() == true)
            //     // {
            //     //     bossHealthHandler.DamageBoss(maxDamageReceived + 5);
            //     // }
            //     // else
            //     // {
            //     //     bossHealthHandler.DamageBoss(0);
            //     // }
            //     damageable.Damage(maxDamageReceived + 5);
            // }
            // else
            //     enemyHealthHandler.Damage(maxDamageReceived + 15);
            
            damageable.Damage(maxDamageReceived + 15);
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
