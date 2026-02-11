using UnityEngine;

public class AfterDoneCredit : MonoBehaviour
{
    public static AfterDoneCredit Instance;
    
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
    }
    private void Start()
    {
        CreditsScrolling.Instance.OnTriggerDoneTextCredit += SetOnAfterCreditPannel;
    }

    public void SetOnAfterCreditPannel()
    {
        gameObject.SetActive(true);
    }

    public void SetOffAfterCreditPannel()
    {
        gameObject.SetActive(false);
    }
}
