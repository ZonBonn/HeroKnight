using UnityEngine;

public class SimplePlayerMoveTest : MonoBehaviour
{
    [SerializeField] float MOVE_SPEED = 3f;
    private Rigidbody2D rb2d;
    float moveDirX;

    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveDirX = 0;
        if (Input.GetKey(KeyCode.A))
        {
            moveDirX = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirX = +1f;
        }
    }

    private void FixedUpdate()
    {
        rb2d.linearVelocity = new Vector2(moveDirX * MOVE_SPEED, rb2d.linearVelocityY);
    }
}
