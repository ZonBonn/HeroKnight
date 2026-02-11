using UnityEngine;

public class DontDestroyOnLoadEventSystem : MonoBehaviour
{
    public static DontDestroyOnLoadEventSystem Instance;

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
}
