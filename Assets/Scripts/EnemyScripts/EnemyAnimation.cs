using UnityEngine;
using System;

public enum EnemyState {Idle, ReadyToCombat, Run, Attack, Jump, Hurt, Death, Fly}
public class EnemyAnimation : MonoBehaviour
{ 
    public Sprite[] IdleSprites;
    public Sprite[] ReadyToCombatSprites;
    public Sprite[] RunSprites;
    public Sprite[] AttackSprites;
    public Sprite[] JumpSprites;
    public Sprite[] RecoverSprites;
    public Sprite[] HurtSprites;
    public Sprite[] DeathSprites;


    private Sprite[] CurrentSprites;
    private int idxBodyFrames;
    private float timerChangeIdxBodyFrames;
    private float m_timerChangeIdxBodyFrameBodyFrame;
    private bool isLoop;

    private SpriteRenderer spriteRenderer;

    public Action<int, Sprite[]> OnChangeEachFrames;
    public Action OnChangeLastFrames;

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

            OnChangeEachFrames?.Invoke(idxBodyFrames, CurrentSprites);
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

            OnChangeLastFrames?.Invoke();
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

    private void ChangetimerChangeIdxBodyFrame(Sprite[] sprites)
    {
        if(sprites == IdleSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == RunSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == AttackSprites)
        {
            timerChangeIdxBodyFrames = 0.1f;
        }
        else if(sprites == JumpSprites)
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
        else if(sprites == ReadyToCombatSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
    }

    public void AnimationHandler(EnemyState state)
    {
        if (state == EnemyState.Idle)
        {
            ChangeAnimation(IdleSprites, true);
        }
        else if (state == EnemyState.Run)
        {
            ChangeAnimation(RunSprites, true);
        }
        else if (state == EnemyState.Jump)
        {
            ChangeAnimation(JumpSprites, false);
        }
        else if (state == EnemyState.Attack)
        {
            ChangeAnimation(AttackSprites, false);
        }
        else if (state == EnemyState.Death)
        {
            ChangeAnimation(DeathSprites, false);
        }
        else if (state == EnemyState.ReadyToCombat)
        {
            ChangeAnimation(ReadyToCombatSprites, true);
        }
        else if (state == EnemyState.Hurt)
        {
            ChangeAnimation(HurtSprites, true);
        }
    }
    
    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }
}

//  Idle        4 frames
//   Combat Idle        4 frames
//   Run       8 frames
//   Attack    8 frames
//   Jump    1 frame
//   Hurt          2 frames
//   Recover    8 frames
//   Death    1 frame