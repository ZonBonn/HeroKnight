using UnityEngine;

public class ProjectileMoving : MonoBehaviour
{
    private Rigidbody2D rb2d;   
    private const float MOVING_SPEED = 5f;
    private ProjectileCollision projectileCollision;

    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        projectileCollision = gameObject.GetComponent<ProjectileCollision>();

        FunctionTimer.Create(DestroySelf, 10f);
    }
    public void Fire(Vector2 dir)
    {
        rb2d.linearVelocity = MOVING_SPEED * dir;
        projectileCollision.setDirBulletLeftOrRight(dir.x);
    }

    private void DestroySelf()
    {
        if(this == null) return; // tránh lỗi khi va chạm sớm hơn thì nó hủy rồi nhưng croutine vẫn chạy tới đây nhưng nó bị xóa mất rồi => lỗi MRE
        Destroy(gameObject);
    }
}
