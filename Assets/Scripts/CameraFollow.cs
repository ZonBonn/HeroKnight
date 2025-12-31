using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform playerTransform;

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
            if(newPosition.x < -8)
            {
                clampedX = -8;
            }
            if(newPosition.x > 52)
            {
                clampedX = 52;
            }
            if(newPosition.y < -2)
            {
                clampedY = -2;
            }
            if(newPosition.y > 26)
            {
                clampedY = 26;
            }
        }
        gameObject.transform.position = new Vector3(clampedX, clampedY, -10f);
    }
}
