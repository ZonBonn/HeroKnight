using UnityEngine;
using System;
using Unity.Collections;

public class ProjectileCollision : MonoBehaviour
{
    private Rigidbody2D rb2d;

    private ProjectileAnimation projectileAnimation;
    private ProjectileMoving projectileMoving;
    private GameObject shooterGameObject;
    private Enemy enemy;

    private float minDamageAttack;
    private float maxDamageAttack;

    private int dirBulletLeftOrRight;


    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        projectileAnimation = gameObject.GetComponentInParent<ProjectileAnimation>();
        projectileMoving = gameObject.GetComponent<ProjectileMoving>();
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
            // Debug.Log("tốc độ x của viên Đạn của EW:" + rb2d.linearVelocityX
            // giảm HP người chơi ở đây
            PlayerHealthStaminaHandler playerHealthStaminaHandler = collider2D.gameObject.GetComponent<PlayerHealthStaminaHandler>();
            PlayerDefense playerDefense = collider2D.gameObject.GetComponent<PlayerDefense>();

            if(playerHealthStaminaHandler != null)
            {
                // playerHealthStaminaHandler.DamageHealth(UnityEngine.Random.Range(minDamageAttack, maxDamageAttack));
                playerDefense.ReceiveDamage(minDamageAttack, maxDamageAttack, dirBulletLeftOrRight);
            }
        }
    }

    public void SetShooter(GameObject shooterGameObject)
    {
        this.shooterGameObject = shooterGameObject;
    }

    public void setDirBulletLeftOrRight(float dirBullet)
    {
        if(dirBullet < 0)
        {
            dirBulletLeftOrRight = -1;
        }
        else if(dirBullet > 0)
        {
            dirBulletLeftOrRight = +1;
        }
        else
        {
            dirBulletLeftOrRight = 0;
        }
    }

    public int getDirBulletLeftOrRight()
    {
        return dirBulletLeftOrRight;
    }
}
