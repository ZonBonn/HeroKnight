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

    private void OnTriggerCallSkill2(Sprite[] sprites) // PrepareSkill2LastFrame Part1: xử lý gọi skill 2 ở đây, nhưng chuyển state sẽ ở bên AI dù cùng logic cuối frame prepareskill2
    {
        if(sprites == bossAnimation.PrepareSkill2Sprites)
        {
            Vector3 playerPosition = Player.Instance.GetPlayerPosition();
            playerPosition.y += 0.7f;
            Instantiate(Skill2PF, playerPosition, Quaternion.identity);

        }
    }
}
