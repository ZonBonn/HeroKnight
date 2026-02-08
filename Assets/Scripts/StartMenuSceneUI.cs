using UnityEngine;
using UnityEngine.UI;

public class StartMenuSceneUI : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;

    private void Start()
    {
        playButton.onClick.AddListener(() => { Loader.Load(Loader.Scene.Level_1); });

        quitButton.onClick.AddListener(() => { Application.Quit(); });
    }
}
