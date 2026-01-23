using UnityEngine;

public class BossCallerSkill2 : MonoBehaviour
{
    public GameObject Skill2PF;
    private BossAnimation bossAnimation;

    private bool CanUseSkill2;
    private float timer;
    private float coolDownSkill2 = 20f;


    private void Awake()
    {
        bossAnimation = gameObject.GetComponent<BossAnimation>();
    }

    private void Start()
    {
        bossAnimation.OnTriggerLastFrames += OnTriggerCallSkill2;

        timer = coolDownSkill2;
    }


    private void Update()
    {
        if(timer > 0)
        {
            CanUseSkill2 = false;
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                CanUseSkill2 = true;
            }
        }
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

    public void SetSkill2CoolDown()
    {
        timer = coolDownSkill2;
    }

    public bool getCanUseSkill2()
    {
        return CanUseSkill2;
    }
}
