using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        playerTransform = PlayerManager.Instance.GetPlayerGameObject().transform;
    }

    private void LateUpdate()
    {
        // for WebGL
        if(playerTransform == null)
        {
            var PlayerTransformVar = PlayerManager.Instance.GetPlayerGameObject();
            if(PlayerTransformVar != null)
            {
                playerTransform = PlayerTransformVar.transform;  // chạy lần 2 khi scene được load để check gán playerTransform
            }
            else
            {
                Debug.Log("PlayerTransformVar == null");
                return;
            }
        }

        Vector3 newPosition = playerTransform.position;
        float clampedX = playerTransform.position.x;
        float clampedY = playerTransform.position.y;
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
        if(!(Input.GetKey(KeyCode.V) && Input.GetMouseButton(1)))
        {
            gameObject.transform.position = new Vector3(clampedX, clampedY, -10f);
        }
        
    }

    public void GetCameraClamp(out float xLeft, out float xRight, out float yUp, out float yDown)
    {
        xLeft = xLeftClamp;
        xRight = xRightClamp;
        yUp = yUpClamp;
        yDown = yDownClamp;
    }

    // for WebGL
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // chạy lần 1 khi scene được load để check gán playerTransform
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryGetPlayer();
    }

    private void TryGetPlayer()
    {
        if(playerTransform == null)
        {
            var playerTransformVar = PlayerManager.Instance.GetPlayerGameObject().transform;
            if(playerTransformVar != null)
            {
                playerTransform = playerTransformVar;
            }
            else
            {
                Debug.Log("playerTransformVar == null");
            }
            return;
        }
        return;
    }
}
