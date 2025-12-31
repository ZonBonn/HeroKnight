using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    public GameObject playerGameObject;

    public Transform cameraTransform;
    public float parallaxFactor = 0.3f;

    private Vector3 lastCamPos;

    private void Start()
    {
        if(playerGameObject == null) // tham chiếu cho các scene tiếp theo mà không được kéo thả thuận tiện như scene1
        {
            playerGameObject = PlayerManager.Instance.GetPlayerGameObject();
        }

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCamPos = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCamPos;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0);
        lastCamPos = cameraTransform.position;
    }
}
