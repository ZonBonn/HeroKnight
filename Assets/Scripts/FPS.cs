using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private float EachTimeFPS = 0.5f;
    private float timer;
    private void Start()
    {
        timer = 0f;
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
