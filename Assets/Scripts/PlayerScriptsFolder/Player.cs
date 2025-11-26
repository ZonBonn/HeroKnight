using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetPlayerPosition()
    {
        return gameObject.transform.position;
    }
}
