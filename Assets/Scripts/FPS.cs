using UnityEngine;

public class FPS : MonoBehaviour

{
    [SerializeField] private float EachTimeFPS = 0.5f;
    public static FPS Instance;
    private float timer;
    private void Start()
    {
        timer = 0f;
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Debug.Log("FPS: " + 1 / Time.deltaTime);
            timer = EachTimeFPS;
        }
    }
}
