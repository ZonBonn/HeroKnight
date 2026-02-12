using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class CreditsScrolling : MonoBehaviour
{
    [SerializeField] GameObject afterDoneCredit;
    // private bool isDoneScrolling = false;

    public static CreditsScrolling Instance;
    
    [SerializeField] float scrollingSpeed = 40f;
    private RectTransform reactTransform;

    // public Action OnTriggerDoneTextCredit;

    public bool isDone = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        reactTransform = gameObject.GetComponent<RectTransform>();
    } 

    private void Update()
    {
        if(reactTransform.anchoredPosition.y >= 0)
        {
            if (!isDone)
            {
                if(afterDoneCredit != null)
                {
                    FunctionTimer.Create(() => { afterDoneCredit.SetActive(true); }, 0.5f);
                }
                // OnTriggerDoneTextCredit?.Invoke();
                // isDoneScrolling = true;
            }
                
            isDone = true;
            return;
        }
        reactTransform.anchoredPosition += Vector2.up * scrollingSpeed * Time.deltaTime;
    }
}
