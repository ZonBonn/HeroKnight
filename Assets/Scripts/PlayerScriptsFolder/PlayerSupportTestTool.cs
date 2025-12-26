using UnityEngine;

public class PlayerSupportTestTool : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerHealthStaminaHandler playerHealthStaminaHandler;
    private PlayerHealthSystem playerHealthSystem; 
    private PlayerAnimation playerAnimation;


    private void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerHealthStaminaHandler = gameObject.GetComponent<PlayerHealthStaminaHandler>();
        playerHealthSystem = playerHealthStaminaHandler.GetPlayerHealthSystem();

        playerAnimation = gameObject.GetComponent<PlayerAnimation>();
    }

    public void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P)) // hoi hp
        {
            playerHealthStaminaHandler.HealHealth(100);
            gameObject.layer = LayerMask.NameToLayer("Player");
            
        }
        if (Input.GetKeyDown(KeyCode.O)) // tu sat
        {
            playerHealthStaminaHandler.DamageHealth(100);
        }
        if (Input.GetKeyDown(KeyCode.I)) // hoi sinh player
        {
            CustomizePlayerRecovery();
        }

        #endif
    }

    public void CustomizePlayerRecovery()
    {
        if(playerMovement.GetPlayerState() == State.Die)
        {
            playerHealthStaminaHandler.HealHealth(100);
            playerMovement.SetPlayerState(State.Idle); // danger setting player state from outside @@@@ nhưng chỉ chạy trong PlayerSupportTestTool thì vẫn chấp nhận được
            gameObject.layer = LayerMask.NameToLayer("Player");
            playerAnimation.AnimationHandler(playerMovement.GetPlayerState());
        }
        

        
    }
}
