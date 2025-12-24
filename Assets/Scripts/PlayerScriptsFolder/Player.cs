using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private PlayerHealthStaminaHandler playerHealthStaminaHandler;
    private PlayerHealthSystem playerHealthSystem;
    private PlayerMovement playerMovement;


    private void Awake()
    {
        Instance = this;
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
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            playerHealthStaminaHandler.HealHealth(100);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            playerHealthStaminaHandler.DamageHealth(100);
        }

        #endif
    }

    public Vector3 GetPlayerPosition()
    {
        return gameObject.transform.position;
    }
}
