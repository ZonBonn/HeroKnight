using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class CreditsScrolling : MonoBehaviour
{
    [SerializeField] GameObject afterDoneCredit;

    public static CreditsScrolling Instance;
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
    [SerializeField] float scrollingSpeed = 40f;
    private RectTransform reactTransform;

    public Action OnTriggerDoneTextCredit;

    public bool isDone = false;

    private void Update()
    {
        if(reactTransform.anchoredPosition.y >= 0)
        {
            if (!isDone)
            {
                OnTriggerDoneTextCredit?.Invoke();
                if(afterDoneCredit != null) afterDoneCredit.SetActive(true);
                // AfterDoneCredit.Instance.SetOnAfterCreditPannel();
            }
                
            isDone = true;
            return;
        }
        reactTransform.anchoredPosition += Vector2.up * scrollingSpeed * Time.deltaTime;
    }
}
