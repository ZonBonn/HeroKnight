using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform playerTransform;

    public float xLeftClamp;
    public float xRightClamp;
    public float yUpClamp;
    public float yDownClamp;

    private void Start()
    {
        if(playerTransform == null) // tham chiếu cho các scene tiếp theo mà không được kéo thả thuận tiện như scene1
        {
            playerTransform = PlayerManager.Instance.GetPlayerGameObject().transform;
        }
    }

    private void LateUpdate()
    {
        Vector3 newPosition = playerTransform.position;
        float clampedX = playerTransform.position.x;
        float clampedY = playerTransform.position.y;
        if(clampedX < -8 || clampedX > 52 || clampedY < -2 || clampedY > 26)
        {
            if(newPosition.x < xLeftClamp)
            {
                clampedX = xLeftClamp;
            }
            if(newPosition.x > xRightClamp)
            {
                clampedX = xRightClamp;
            }
            if(newPosition.y < yDownClamp)
            {
                clampedY = yDownClamp;
            }
            if(newPosition.y > yUpClamp)
            {
                clampedY = yUpClamp;
            }
        }
        gameObject.transform.position = new Vector3(clampedX, clampedY, -10f);
    }
}
