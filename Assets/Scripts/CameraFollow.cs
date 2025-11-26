using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform playerTransform;

    private void LateUpdate()
    {
        gameObject.transform.position = new Vector3(playerTransform.transform.position.x, playerTransform.transform.position.y, -10f);
    }
}
