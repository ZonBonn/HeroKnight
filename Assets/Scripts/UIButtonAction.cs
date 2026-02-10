using UnityEngine;

public class UIButtonAction : MonoBehaviour
{
    public static UIButtonAction Instance;
    [SerializeField] GameObject diePannel;
    [SerializeField] GameObject pausedPannel; 

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        // else
        // {
        //     Destroy(gameObject);
        //     return;
        // }
    }

    public void Home()
    {
        Loader.Load(Loader.Scene.StartMenu);
    }

    public void ReplayButton()
    {
        Loader.ReloadLevel();
    }

    public void PausedButton()
    {
        if(pausedPannel.activeSelf == true)
        {
            UICanvasManager.Instance.HidePannel(pausedPannel);
            ResumeGame();
        }
        else
        {
            UICanvasManager.Instance.ShowPannel(pausedPannel);
            PauseGame();
        }
        
    }

    public void XPausedPannelButton()
    {
        UICanvasManager.Instance.HidePannel(pausedPannel);
        ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
