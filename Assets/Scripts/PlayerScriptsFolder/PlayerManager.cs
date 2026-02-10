using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private GameObject PlayerGameObject;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public GameObject GetPlayerGameObject()
    {
        return PlayerGameObject;
    }

    public void RegisterPlayer(GameObject PlayerGameObject)
    {
        this.PlayerGameObject = PlayerGameObject;
    }
}
