using UnityEngine;

public class ProjectileMoving : MonoBehaviour
{
    private Rigidbody2D rb2d;   
    private const float MOVING_SPEED = 5f;

    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }
    public void Fire(Vector2 dir)
    {
        rb2d.linearVelocity = MOVING_SPEED * dir;
    }
}
