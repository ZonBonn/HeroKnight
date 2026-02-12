using UnityEngine;

public class UIButtonAction : MonoBehaviour
{
    public static UIButtonAction Instance;
    [SerializeField] GameObject diePannel;
    [SerializeField] GameObject pausedPannel;
    [SerializeField] GameObject instructionPannel;

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
        Debug.Log("UIButtonAction ID:" + Instance.GetHashCode());
    }

    private void Update()
    {
        
    }

    public void Home()
    {
        ResumeGame();
        Loader.Load(Loader.Scene.Start_Menu);
    }

    public void ReplayButton()
    {
        ResumeGame();
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

    public void InstructionButton()
    {
        if(instructionPannel.activeSelf == true)
        {
            UICanvasManager.Instance.HidePannel(instructionPannel);
        }
        else
        {
            UICanvasManager.Instance.ShowPannel(instructionPannel);
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
