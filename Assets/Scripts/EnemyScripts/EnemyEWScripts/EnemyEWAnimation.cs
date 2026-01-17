using UnityEngine;
using System;
public enum EnemyEWState{
Idle, 
Walk, 
Run, 
Jump, 
Fall, 
Attack, 
Hurt, 
Die, 
Recovery
}

public class EnemyEWAnimation : MonoBehaviour
{
    public Sprite[] IdleSprites;
    public Sprite[] WalkSprites;
    public Sprite[] RunSprites;
    public Sprite[] JumpSprites;
    public Sprite[] FallSprites;
    public Sprite[] AttackSprites;
    public Sprite[] HurtSprites;
    public Sprite[] DeathSprites;
    public Sprite[] RecoverSprites;

    private Sprite[] CurrentSprites;
    private int idxBodyFrames;
    private float timerChangeIdxBodyFrames;
    private float m_timerChangeIdxBodyFrameBodyFrame;
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
    }

    private void PlayAnimation()
    {
        m_timerChangeIdxBodyFrameBodyFrame -= Time.deltaTime;
        if(m_timerChangeIdxBodyFrameBodyFrame <= 0)
        {
            spriteRenderer.sprite = CurrentSprites[idxBodyFrames++];
            m_timerChangeIdxBodyFrameBodyFrame = timerChangeIdxBodyFrames;

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
        m_timerChangeIdxBodyFrameBodyFrame = 0;
        CurrentSprites = sprites;
        this.isLoop = isLoop;
        ChangetimerChangeIdxBodyFrame(sprites);
    }

    // SAU NÀY THI CÓ THỂ DÙNG DICTIONARY, HOẶC LIST CHO DATA TỰ ĐỌC ĐỂ NÓ QUYẾT ĐỊNH, LÀM NÀY HƠI "TRÂU" NẾU SAU THÊM ANIMATION
    private void ChangetimerChangeIdxBodyFrame(Sprite[] sprites)
    {
        if(sprites == IdleSprites)
        {
            timerChangeIdxBodyFrames = 0.1f;
        }
        else if(sprites == WalkSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == RunSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == JumpSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == FallSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == AttackSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == HurtSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == DeathSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == RecoverSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
    }

    public void AnimationHandler(EnemyEWState state)
    {
        if (state == EnemyEWState.Idle)
        {
            ChangeAnimation(IdleSprites, true);
        }
        else if (state == EnemyEWState.Walk)
        {
            ChangeAnimation(RunSprites, true);
        }
        else if (state == EnemyEWState.Run)
        {
            ChangeAnimation(RunSprites, true);
        }
        else if (state == EnemyEWState.Jump)
        {
            ChangeAnimation(JumpSprites, false);
        }
        // Idle walk run jump fall attack hurt death
        else if (state == EnemyEWState.Fall)
        {
            ChangeAnimation(FallSprites, false);
        }
        else if (state == EnemyEWState.Attack)
        {
            ChangeAnimation(AttackSprites, false);
        }
        else if (state == EnemyEWState.Hurt)
        {
            ChangeAnimation(HurtSprites, false);
        }
        else if (state == EnemyEWState.Die)
        {
            ChangeAnimation(DeathSprites, false);
        }
        else if (state == EnemyEWState.Recovery)
        {
            ChangeAnimation(RecoverSprites, false);
        }
    }
    
    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }
}
