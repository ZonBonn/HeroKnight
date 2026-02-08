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

    public static Action OnSceneCallback;

    public static void Load(Scene scene)
    {
        OnSceneCallback = () => {
            SceneManager.LoadScene(scene.ToString());
        };

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
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
