using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private CameraFollow cameraFollow;
    private float xLeftClamp;
    private float xRightClamp;
    private float yUpClamp;
    private float yDownClamp;

    private Vector3 lastMousePosition;
    [SerializeField] float dragSpeed = 1f;

    private void Awake()
    {
        cameraFollow = gameObject.GetComponent<CameraFollow>();
        cameraFollow.GetCameraClamp(out float xLeft, out float xRight, out float yUp, out float yDown);
        xLeftClamp = xLeft;
        xRightClamp = xRight;
        yUpClamp = yUp;
        yDownClamp = yDown;
    }

    private void LateUpdate()
    {
        CameraDragHandler();
    }

    private void CameraDragHandler()
    {
        
        if(Input.GetKey(KeyCode.V) && Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if(Input.GetKey(KeyCode.V) && Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            
            ClampDragCamera(delta);

            lastMousePosition = Input.mousePosition;
        }
    }

    private void ClampDragCamera(Vector3 delta)
    {
        Vector3 newPosition = gameObject.transform.position - delta * dragSpeed * Time.deltaTime;
        float clampedX = newPosition.x;
        float clampedY = newPosition.y;
        if(clampedX < xLeftClamp || clampedX > xRightClamp || clampedY < yDownClamp || clampedY > yUpClamp)
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
