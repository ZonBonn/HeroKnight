using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class Loader
{
    public enum Scene
    {
        StartMenu,
        Level_1,
        Level_2,
        Last_Level,
        LoadingScene,
    }

    public static Action OnSceneCallback; // gọi load màn tiếp theo

    public static void Load(Scene scene)
    {
        // đăng ký hàm gọi scene tiếp theo để khi nào tới loading scene thì loading scene sẽ gọi cái màn mà cần tới
        OnSceneCallback = () => {
            FunctionTimer.Create(() => { SceneManager.LoadSceneAsync(scene.ToString()); }, 1f);
            
        };

        // OnSceneCallback = () => { SceneManager.LoadScene(scene.ToString()); }; 

        SceneManager.LoadSceneAsync(Scene.LoadingScene.ToString());
    }

    public static void sceneCallback()
    {
        if(OnSceneCallback != null)
        {
            OnSceneCallback?.Invoke();
            OnSceneCallback = null;

        }
    }
}
