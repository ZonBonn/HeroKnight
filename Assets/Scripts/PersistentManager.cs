using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance;

    [SerializeField] List<GameObject> listPersistentGO;
    
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMod)
    {
        if(scene.name == "LoadingScene" || scene.name == "StartMenu")
        {
            SetActiveOffAllGO();
        }
        else
        {
            SetActiveOnAllGO();
        }
    }

    private void SetActiveOffAllGO()
    {
        if(listPersistentGO == null) return;
        for(int i = 0 ; i < listPersistentGO.Count ; i++)
        {
            listPersistentGO[i].SetActive(false);
        }
    }

    private void SetActiveOnAllGO()
    {
        if(listPersistentGO == null) return;
        for(int i = 0 ; i < listPersistentGO.Count ; i++)
        {
            listPersistentGO[i].SetActive(true);
        }
    }
}
