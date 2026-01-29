using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    private Rigidbody2D rb2d;

    private ProjectileAnimation projectileAnimation;
    private GameObject shooterGameObject;
    private Enemy enemy;

    private float minDamageAttack;
    private float maxDamageAttack;


    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        projectileAnimation = gameObject.GetComponentInParent<ProjectileAnimation>();
    }

    private void Start()
    {
        enemy = shooterGameObject.GetComponent<Enemy>();
        enemy.getFeature(out float minDamageReceived, out float maxDamageReceived, out float minDamageAttack, out float maxDamageAttack);
        this.minDamageAttack = minDamageAttack;
        this.maxDamageAttack = maxDamageAttack;
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        rb2d.linearVelocity = Vector3.zero;// khi va chạm thì thôi không di chuyển nữa 
        projectileAnimation.ProjectileAnimationHandler(ProjectileState.Explode); // sau cái này thì nó sẽ tự hủy mà 

        if (collider2D.CompareTag("Player"))
        {
            // giảm HP người chơi ở đây
            PlayerHealthStaminaHandler playerHealthStaminaHandler = collider2D.gameObject.GetComponent<PlayerHealthStaminaHandler>();

            if(playerHealthStaminaHandler != null)
            {
                playerHealthStaminaHandler.DamageHealth(UnityEngine.Random.Range(minDamageAttack, maxDamageAttack));
            }
        }
    }

    public void SetShooter(GameObject shooterGameObject)
    {
        this.shooterGameObject = shooterGameObject;
    }
}
