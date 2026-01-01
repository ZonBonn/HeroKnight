using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; } // lưu chữ instance (chính là cái component của người chơi (mỗi 1 component là một instance), để có thể gọi cái component này ở bất kì đâu)

    private PlayerHealthStaminaHandler playerHealthStaminaHandler;
    private PlayerHealthSystem playerHealthSystem;
    private PlayerMovement playerMovement;



    private void Awake()
    {
        
        if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this; // this == trỏ tới component được gán trong Object trong scene
        }
        else
        {
            Destroy(gameObject);
        }
            
        PlayerManager.Instance.RegisterPlayer(gameObject); // gửi PlayerGameObject đi cho các scene tiếp theo
    }
    private void Start()
    {
        playerHealthStaminaHandler = gameObject.GetComponent<PlayerHealthStaminaHandler>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerHealthSystem = playerHealthStaminaHandler.GetPlayerHealthSystem();


    }

    // TEST FUNCTION
    private void Update()
    {

    }

    public Vector3 GetPlayerPosition()
    {
        return gameObject.transform.position;
    }
}
