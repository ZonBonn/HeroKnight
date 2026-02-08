using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        FunctionTimer.Create(Action, 3f, "timer 1");
        FunctionTimer.Create(Action2, 5f, "timer 2");
        FunctionTimer.StopTimer("timer 1");
    }

    private void Action()
    {
        Debug.Log("Test !");
    }

    private void Action2()
    {
        Debug.Log("Test 2!");
    }
}
