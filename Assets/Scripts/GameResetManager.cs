using System.Collections.Generic;
using UnityEngine;

public class GameResetManager : MonoBehaviour
{
    public static GameResetManager Instance;

    public List<GameObject> resettableGameObject;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Start()
    {
        Loader.OnSeneReloaded += ResetAllResettableGameObject;
        Loader.OnSceneLoaded += ResetSceneByScene;
    }

    public void ResetAllResettableGameObject()
    {
        for(int i = 0 ; i < resettableGameObject.Count ; i++)
        {
            IResettable resettable = null;
            if(resettableGameObject[i] != null)
                resettable = resettableGameObject[i].GetComponent<IResettable>();
            if(resettable != null)
                resettable.ResetState();
        }
    }

    public void ResetSceneByScene(Loader.Scene scene)
    {
        if(scene == Loader.Scene.Start_Menu) // nếu load lại start menu thì reset, câu lệnh kia thì chỉ reset khi reload, giờ reload khi về menu nữa
        {
            for(int i = 0 ; i < resettableGameObject.Count ; i++)
            {
                IResettable resettable = null;
                if(resettableGameObject[i] != null)
                    resettable = resettableGameObject[i].GetComponent<IResettable>();
                if(resettable != null)
                    resettable.ResetState();
            }
        }
    }
}
