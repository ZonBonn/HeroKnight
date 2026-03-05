using UnityEngine;

public class Boss : MonoBehaviour, IResettable
{
    private BossHealthHandler bossHealthHandler;

    private void Awake()
    {
        bossHealthHandler = gameObject.GetComponent<BossHealthHandler>();

        
    }

    private void OnEnable()
    {
        GameResetManager.Instance.resettableGameObject.Add(this.gameObject);
    }
    private void OnDisable() 
    {
        GameResetManager.Instance.resettableGameObject.Remove(this.gameObject);
    }

    public void ResetState()
    {
        bossHealthHandler.Heal(100);
    }
}
