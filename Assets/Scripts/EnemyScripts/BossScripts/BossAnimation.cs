using UnityEngine;
using System;
using UnityEngine.Rendering;

public enum BossState
{
    Idle,
    Walk,
    Attack,
    Death,
    Hurt,

    InvisibleSkill1Sprites, // visible
    Visible,

    Skill2, // far attack distance
    PrepareSkill2,
    KeeppInvisible,
}
public class BossAnimation : MonoBehaviour
{
    public Sprite[] IdleSprites; // 0-7
    public Sprite[] WalkSprites; // 8-15
    public Sprite[] AttackSprites; // 16 -25
    public Sprite[] DeathSprites; // 29 - 38
    public Sprite[] HurtSprites; // 26-28

    public Sprite[] InvisibleSkill1Sprites; // 29 - 38
    public Sprite[] VisibleSprites; // 38 - 29

    public Sprite[] Skill2Sprites; // 48 - 63 cái này sẽ dành cho skill riêng
    public Sprite[] PrepareSkill2Sprites; // 39 - 47
    public Sprite[] KeepInvisibleSprites; // just 1 frame tàng hình
    

    private Sprite[] CurrentSprites;
    private int idxBodyFrames;
    private float timerChangeIdxBodyFrames;
    private float m_timerChangeIdxBodyFrame;
    private bool isLoop;

    private SpriteRenderer spriteRenderer;

    public Action<int, Sprite[]> OnTriggerEachFrames;
    public Action<Sprite[]> OnTriggerLastFrames;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ChangeAnimation(IdleSprites, true);
    }

    private void Update()
    {
        PlayAnimation();

        // Test
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     AnimationHandler(BossState.PrepareSkill2);
        // }
    }

    private void PlayAnimation()
    {
        m_timerChangeIdxBodyFrame -= Time.deltaTime;
        if(m_timerChangeIdxBodyFrame <= 0)
        {
            spriteRenderer.sprite = CurrentSprites[idxBodyFrames++];
            m_timerChangeIdxBodyFrame = timerChangeIdxBodyFrames;

            OnTriggerEachFrames?.Invoke(idxBodyFrames, CurrentSprites);
        }
        if (idxBodyFrames == CurrentSprites.Length) // đoạn này thực hiện khi chạy m_timerChangeIdxBodyFrameBodyFrame của frame cuối cùng rồi
        {
            if(isLoop == true)
            {
                idxBodyFrames = 0;
            }
            else
            {
                idxBodyFrames = CurrentSprites.Length-1; // stop at the last frame if isLoop == false
            }

            OnTriggerLastFrames?.Invoke(CurrentSprites);
        }
    } 

    private void ChangeAnimation(Sprite[] sprites, bool isLoop)
    {
        if(sprites == CurrentSprites)
        {
            return;
        }

        idxBodyFrames = 0;
        m_timerChangeIdxBodyFrame = 0;
        CurrentSprites = sprites;
        this.isLoop = isLoop;
        ChangetimerChangeIdxBodyFrame(sprites);
    }

    // SAU NÀY THI CÓ THỂ DÙNG DICTIONARY, HOẶC LIST CHO DATA TỰ ĐỌC ĐỂ NÓ QUYẾT ĐỊNH, LÀM NÀY HƠI "TRÂU" NẾU SAU THÊM ANIMATION
    private void ChangetimerChangeIdxBodyFrame(Sprite[] sprites)
    {
        if(sprites == IdleSprites)
        {
            timerChangeIdxBodyFrames = 0.12f;
        }
        else if(sprites == WalkSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == AttackSprites)
        {
            timerChangeIdxBodyFrames = 0.1f;
        }
        else if(sprites == DeathSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == HurtSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == InvisibleSkill1Sprites)
        {
            timerChangeIdxBodyFrames = 0.09f;
        }
        else if(sprites == VisibleSprites)
        {
            timerChangeIdxBodyFrames = 0.09f;
        }
        else if(sprites == PrepareSkill2Sprites)
        {
            timerChangeIdxBodyFrames = 0.1f;
        }
        else if (sprites == KeepInvisibleSprites)
        {
            timerChangeIdxBodyFrames = float.MaxValue;
        }
    }

    // Idle, Walk, Attack, Death, Hurt, Skill1, Visible, Skill2, PrepareSkill2
    public void AnimationHandler(BossState state)
    {
        if (state == BossState.Idle)
        {
            ChangeAnimation(IdleSprites, true);
        }
        else if (state == BossState.Walk)
        {
            ChangeAnimation(WalkSprites, true);
        }
        else if (state == BossState.Attack)
        {
            ChangeAnimation(AttackSprites, true);
        }
        else if (state == BossState.Death)
        {
            ChangeAnimation(DeathSprites, false);
        }
        // Idle walk run jump fall attack hurt death
        else if (state == BossState.Hurt)
        {
            ChangeAnimation(HurtSprites, false);
        }
        else if (state == BossState.InvisibleSkill1Sprites)
        {
            ChangeAnimation(InvisibleSkill1Sprites, false);
        }
        else if (state == BossState.Visible)
        {
            ChangeAnimation(VisibleSprites, false);
        }
        else if (state == BossState.Skill2)
        {
            ChangeAnimation(Skill2Sprites, false);
        }
        else if (state == BossState.PrepareSkill2)
        {
            ChangeAnimation(PrepareSkill2Sprites, false);
        }
        else if (state == BossState.KeeppInvisible)
        {
            ChangeAnimation(KeepInvisibleSprites, true);
        }
    }
    
    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }
}
// pivot custom x = 0.7571428
// pivot custom y = 0.3010753
