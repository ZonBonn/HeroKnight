using UnityEngine;

public class BossCallerSkill2 : MonoBehaviour
{
    public GameObject Skill2PF;
    private BossAnimation bossAnimation;


    private void Awake()
    {
        bossAnimation = gameObject.GetComponent<BossAnimation>();
    }

    private void Start()
    {
        bossAnimation.OnTriggerLastFrames += OnTriggerCallSkill2;
    }


    private void Update()
    {
        
    }

    private void OnTriggerCallSkill2(Sprite[] sprites)
    {
        if(sprites == bossAnimation.PrepareSkill2Sprites)
        {
            Vector3 playerPosition = Player.Instance.GetPlayerPosition();
            playerPosition.y += 0.7f;
            GameObject skill2 = Instantiate(Skill2PF, playerPosition, Quaternion.identity);

        }
    }
}
