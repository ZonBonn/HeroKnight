using UnityEngine;
using System;
using System.Collections.Generic;

public class FunctionTimer
{
    private static List<FunctionTimer> activeFunctionTimer;
    private static GameObject initGameObject; // đánh dấu được khởi tạo rồi chứ ? không check List == null mà GameObject hoạt động đúng theo vòng đời Unity

    private static void InitIfNeeded()
    {
        if(initGameObject == null)
        {
            activeFunctionTimer = new List<FunctionTimer>();
            initGameObject = new GameObject("FunctionTimer_InitGameObject");

        }
    }

    private static void AddTimer(FunctionTimer functionTimer)
    {
        activeFunctionTimer.Add(functionTimer);
    }

    private static void RemoveTimer(FunctionTimer functionTimer)
    {
        InitIfNeeded();
        activeFunctionTimer.Remove(functionTimer);
    }

    public static void StopTimer(string timerName)
    {
        for(int i = 0 ; i < activeFunctionTimer.Count ; i++)
        {
            if(activeFunctionTimer[i].timerName  == timerName)
            {
                activeFunctionTimer[i].DestroySelf();
                --i;
            }
        }
    }

    public static FunctionTimer Create(Action action, float timer, string timerName = null)
    {
        InitIfNeeded(); // khởi tạo phát đầu List, nếu khởi tạo rồi thì cũng gọi cũng không chạy

        GameObject functionTimerGameObject = new GameObject("Function Timer", typeof(MonoBehaviourHook)); // tạo GameObject có tên Function Timer và gán component MonoBehaviourHook cho nó

        FunctionTimer functionTimer = new FunctionTimer(action, timer, timerName, functionTimerGameObject);
        
        functionTimerGameObject.GetComponent<MonoBehaviourHook>().onUpdate = functionTimer.Update;

        AddTimer(functionTimer);

        return functionTimer;
    }

    private class MonoBehaviourHook : MonoBehaviour // cho hàm Update của class FunctionTimer chạy trong Update này khi sử dụng, không phải chạy trong Update của class sử dụng FunctionTimer nữa
    {
        public Action onUpdate;
        private void Update() // Update này sẽ chạy thay Update của đối tượng gọi FunctionTimer (nghĩa là Update của class gọi FunctionTimer sẽ không cần gọi Update của FunctionTimer nữa mà cái Update của MonoBehaviourHook sẽ gọi thay => nhỏ gọn tiện hơn code nhìn clean hơn)
        {
            if(onUpdate != null) onUpdate.Invoke();
        }
    }
    
    private Action action;
    private float timer;
    private string timerName;
    private GameObject functionTimerGameObject;
    private bool isDestroyed; // chỉ chạy action 1 lần duy nhất

    private FunctionTimer(Action action, float timer, string timerName, GameObject functionTimerGameObject)
    {
        this.action = action;
        this.timer = timer;
        this.timerName = timerName;
        this.functionTimerGameObject = functionTimerGameObject;
        isDestroyed = false;
    }

    private void Update()
    {
        if(isDestroyed != true)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                action?.Invoke();
                DestroySelf();
            }
        } 
    }

    private void DestroySelf()
    {
        isDestroyed = true;
        UnityEngine.Object.Destroy(functionTimerGameObject);
        RemoveTimer(this);
    }
}
