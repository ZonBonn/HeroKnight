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
    }

    public void ResetAllResettableGameObject()
    {
        for(int i = 0 ; i < resettableGameObject.Count ; i++)
        {
            IResettable resettable = resettableGameObject[i].GetComponent<IResettable>();
            resettable.ResetState();
        }
    }
}
