using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    private PlayerMovement playerMovement;
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    
    void Update()
    {
        
    }
}
