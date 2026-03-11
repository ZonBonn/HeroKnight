using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private GameObject playerGameObject;
    private void OnEnable()
    {
        if(playerGameObject == null)
        {
            playerGameObject = PlayerManager.Instance.GetPlayerGameObject();
        }
        playerGameObject.transform.position = gameObject.transform.position;
    }
}
