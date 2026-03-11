using UnityEngine;

public class BossSupportTestTool : MonoBehaviour
{
    private BossHealthHandler bossHealthHandler;
    private BossAnimation bossAnimation;
    private bool canKillTheBoss = false;

    private void Awake()
    {
        bossHealthHandler = gameObject.GetComponent<BossHealthHandler>();
        bossAnimation = gameObject.GetComponent<BossAnimation>();
    }

    private void Start()
    {
        bossAnimation.OnTriggerLastFrames += OnBossApear;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U) && canKillTheBoss == true)
        {
            bossHealthHandler.DamageBoss(100);
        }
    }

    private void OnBossApear(Sprite[] sprites)
    {
        if(sprites == bossAnimation.WaitToFightSprites)
            canKillTheBoss = true;
    }
}
