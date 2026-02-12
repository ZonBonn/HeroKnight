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
    private void OnEnable()
    {
        // CreditsScrolling.Instance.OnTriggerDoneTextCredit += SetOnAfterCreditPannel;
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Loader.Load(Loader.Scene.Start_Menu);
            // UICanvasManager.Instance.HidePannel(UICanvasManager.Instance.endingPannel);
        }
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
