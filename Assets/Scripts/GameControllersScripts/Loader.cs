using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Unity.VisualScripting;

public static class Loader
{
    public class DummyMonoBehaviourClass : MonoBehaviour { } // StartCoroutine thuộc class MonoBehaviour => cần class giả này để chạy coroutine
    private static AsyncOperation asyncOperation;

    public static Action OnSeneReloaded;

    public enum Scene // phải giống trong build setting scene litst
    {
        StartMenu,
        LoadingScene,
        Level_1,
        Level_2,
        Last_Level,
    }

    public static Action OnLoaderCallback; // gọi load màn tiếp theo

    public static void Load(Scene scene)
    {
        asyncOperation = null;

        // đăng ký hàm gọi scene tiếp theo để khi nào tới loading scene thì loading scene sẽ gọi cái màn mà cần tới
        // ĐĂNG KÝ CHỨ KHÔNG PHẢI LOAD SCENE CHÍNH NÀY
        OnLoaderCallback = () => {
            // FunctionTimer.Create(() => { SceneManager.LoadSceneAsync(scene.ToString()); }, 1f);
            GameObject gameObject = new GameObject("Loading Game Object");
            gameObject.AddComponent<DummyMonoBehaviourClass>();
            
            // FunctionTimer.Create(() => { asyncOperation = SceneManager.LoadSceneAsync(scene.ToString()); }, 1f);
            
            gameObject.GetComponent<DummyMonoBehaviourClass>().StartCoroutine(LoadingSceneASync(scene, gameObject));
            
        };

        // OnSceneCallback = () => { SceneManager.LoadScene(scene.ToString()); }; 

        SceneManager.LoadSceneAsync(Scene.LoadingScene.ToString());
    }

    public static void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        Scene nextScene = (Scene)nextSceneIndex; // ép sang giá trị kế bên phải của scene hiện tại
        // Debug.Log("Load Level" + nextScene);

        Load(nextScene);
    }

    
    public static void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Load scene:" + (Scene)currentSceneIndex);
        OnSeneReloaded?.Invoke();
        Load((Scene)currentSceneIndex);
    }
    
    public static void LoaderCallback()
    {
        if(OnLoaderCallback != null)
        {
            OnLoaderCallback?.Invoke(); // tới loading scene thì bắt đầu load scene chính
            OnLoaderCallback = null;

        }
    }

    static IEnumerator LoadingSceneASync(Scene scene, GameObject gameObject)
    {
        yield return new WaitForSeconds(0.5f);
        asyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        asyncOperation.allowSceneActivation = false; // chặn khi tải xong thì chưa cho bật scene
        
        while (asyncOperation.progress < 0.9f)
        {
            // Debug.Log(asyncOperation.progress);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // tải xong thì đợi 1s mới load vào scene chính

        // Cho phép chuyển scene
        asyncOperation.allowSceneActivation = true;
        GameObject.Destroy(gameObject);
    }

    public static float getLoadProgress()
    {
        if(asyncOperation != null)
        {
            if(asyncOperation.progress < 0.9f)
            {
                return asyncOperation.progress;
            }
            else
            {
                return 1f;
            }
        }
        else
        {
            return 0f;
        }
    }
}
