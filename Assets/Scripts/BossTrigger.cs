using UnityEngine;
using System;

public class BossTrigger : MonoBehaviour
{
    public static Action OnTriggerBossStartFighting; // hay luôn cho nó là static thì đăng ký được bên ngoài luôn không cần phải tham chiếu tới nữa HAY
    private bool isStart = false;

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        Debug.Log("Hit something ?");
        if(collider2D.CompareTag("Player") == true && isStart == false)
        {
            OnTriggerBossStartFighting?.Invoke();
            isStart = true;
        }
    }
}
