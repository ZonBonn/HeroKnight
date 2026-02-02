using UnityEngine;
using System;

public class BossTrigger : MonoBehaviour
{
    public static Action OnTriggerBossStartFighting; // hay luôn cho nó là static thì đăng ký được bên ngoài luôn không cần phải tham chiếu tới nữa HAY
    private bool isStart = false;

    private void Start()
    {
        UICanvasManager.Instance.RegisterBossStartFighting();
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        // Debug.Log("Hit something ?");
        if(collider2D.CompareTag("Player") == true && isStart == false)
        {
            FunctionTimer.Create(OnTriggerBossStartFighting, 0.5f);
            // OnTriggerBossStartFighting?.Invoke();
            isStart = true;
        }
    }
}
