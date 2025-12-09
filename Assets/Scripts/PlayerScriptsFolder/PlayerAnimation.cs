using UnityEngine;
using System;

public class PlayerAnimation : MonoBehaviour
{
    public Sprite[] IdleSprites;
    public Sprite[] RunSprites;
    public Sprite[] Attack1Sprites;
    public Sprite[] Attack2Sprites;
    public Sprite[] Attack3Sprites;
    public Sprite[] JumpSprites;
    public Sprite[] FallSprites;
    public Sprite[] HurtSprites;
    public Sprite[] DeathSprites;
    public Sprite[] BlockIdleSprites;
    public Sprite[] BlockHitSprites;
    public Sprite[] RollSprites;
    public Sprite[] ClimbingToWallSprites;
    public Sprite[] HoldingWallSprites;

    private Sprite[] CurrentSprites;
    private int idxBodyFrames;
    private float timerChangeIdxBodyFrames;
    private float m_timerChangeIdxBodyFrameBodyFrame;
    private bool isLoop;

    private SpriteRenderer spriteRenderer;

    public Action<int, Sprite[]> OnChangeEachFrames; // chạy mọi frame của sprites (int [1 -> Sprite[].Length])
    public Action<Sprite[]> OnChangeLastFrames; // chạy frame sau frame cuối cùng của sprites

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

            OnChangeEachFrames?.Invoke(idxBodyFrames, CurrentSprites); // idxBodyFrames = [1-> CurrentSprites.Length]
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

            OnChangeLastFrames?.Invoke(CurrentSprites);
        }
    } 

    public void ChangeAnimation(Sprite[] sprites, bool isLoop)
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
            timerChangeIdxBodyFrames = 0.1f;
        }
        else if(sprites == Attack1Sprites)
        {
            timerChangeIdxBodyFrames = 0.06f;
        }
        else if(sprites == Attack2Sprites)
        {
            timerChangeIdxBodyFrames = 0.06f;
        }
        else if(sprites == Attack3Sprites)
        {
            timerChangeIdxBodyFrames = 0.06f;
        }
        else if(sprites == JumpSprites)
        {
            timerChangeIdxBodyFrames = 0.1f;
        }
        else if(sprites == FallSprites)
        {
            timerChangeIdxBodyFrames = 0.15f;
        }
        else if(sprites == HurtSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == DeathSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == BlockIdleSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == BlockHitSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == RollSprites)
        {
            timerChangeIdxBodyFrames = 0.1f;
        }
        else if(sprites == HoldingWallSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
        else if(sprites == ClimbingToWallSprites)
        {
            timerChangeIdxBodyFrames = 0.2f;
        }
    }

    public void AnimationHandler(State state)
    {
        if (state == State.Idle)
        {
            ChangeAnimation(IdleSprites, true);
        }
        else if (state == State.Run)
        {
            ChangeAnimation(RunSprites, true);
        }
        else if (state == State.Jump)
        {
            ChangeAnimation(JumpSprites, false);
        }
        else if (state == State.Attack1) // attack 1, 2, 3
        {
            ChangeAnimation(Attack1Sprites, false);
        }
        else if (state == State.Attack2) // attack 1, 2, 3
        {
            ChangeAnimation(Attack2Sprites, false);
        }
        else if (state == State.Attack3) // attack 1, 2, 3
        {
            ChangeAnimation(Attack3Sprites, false);
        }
        else if (state == State.Roll)
        {
            ChangeAnimation(RollSprites, false);
        }
        else if (state == State.ClimbingOnWall)
        {
            ChangeAnimation(HoldingWallSprites, true);
        }
        else if (state == State.Fall)
        {
            ChangeAnimation(FallSprites, false);
        }
        else if (state == State.BlockIdle)
        {
            ChangeAnimation(BlockIdleSprites, true);
        }
        else if (state == State.Die)
        {
            ChangeAnimation(DeathSprites, false);
        }
    }
    
    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }
}

public class PlayerAnimationOld : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] IdleSprites; // all is lacked "s"
    public Sprite[] RunSprites;
    public Sprite[] Attack1Sprites;
    public Sprite[] Attack2Sprites;
    public Sprite[] Attack3Sprites;
    public Sprite[] DieSprites;
    public Sprite[] JumpSprites; // theo animation jump là 38-44 nhưng nếu thêm những animation fall thì đẹp hơn ok
    public Sprite[] RollSprites;
    public Sprite[] ClimbingOnWallSprites;
    public Sprite[] ActionOnWallSprites;
    public Sprite[] FallSprites;
    public Sprite[] HitSprites;
    public Sprite[] BlockIdleSprites;
    public Sprite[] BlockHitSprites;
    private Sprite[] currentSprites;
    private int idxFrame = 0;
    private float eachFrameTime = 0.2f;
    private float timer = 0f;
    private bool isLoop;
    public Action OnLastFrameOfSpritesNoLoop;
    public Action<int, Sprite[]> OnChangeIDXFrame;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // timerGeneric = gameObject.GetComponent<TimerGeneric>();
    }

    private void Start()
    {
        ChangeAnimation(IdleSprites, true);
    }

    // C1:
    private void Update()
    {
        PlayAnimation();
    }

    public void ChangeAnimation(Sprite[] anim, bool isLoop)
    {
        if (anim == currentSprites) // elimination same animation (nếu trùng animation thì thôi bỏ qua khong cho chạy lại từ đầu)
            return;
        currentSprites = anim;
        this.isLoop = isLoop;
        ChangeEachFrameTimeBySprites(anim);
        timer = eachFrameTime;
        idxFrame = 0;
    }

    private void PlayAnimation() // chỉ chạy currentSprites
    {
        spriteRenderer.sprite = currentSprites[idxFrame];
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            OnChangeIDXFrame.Invoke(idxFrame, currentSprites);
            ++idxFrame; // invoke mỗi frame được thay đổi
            if (isLoop == true)
            {
                idxFrame %= currentSprites.Length;
            }
            else if (isLoop == false && idxFrame >= currentSprites.Length)
            {
                OnLastFrameOfSpritesNoLoop?.Invoke(); // invoke frame cuối để báo hết animation
                idxFrame %= currentSprites.Length;
            }
            timer = eachFrameTime;
        }
    }

    

    private void ChangeEachFrameTimeBySprites(Sprite[] sprites)
    {
        if (sprites == IdleSprites)
        {
            eachFrameTime = 0.2f;
        }
        else if (sprites == RunSprites)
        {
            eachFrameTime = 0.1f;
        }
        else if (sprites == Attack1Sprites)
        {
            eachFrameTime = 0.05f;
        }
        else if (sprites == Attack2Sprites)
        {
            eachFrameTime = 0.05f;
        }
        else if (sprites == Attack3Sprites)
        {
            eachFrameTime = 0.05f;
        }
        else if (sprites == DieSprites)
        {
            eachFrameTime = 0.2f;
        }
        else if (sprites == JumpSprites)
        {
            eachFrameTime = 0.1f;
        }
        else if (sprites == RollSprites)
        {
            eachFrameTime = 0.1f;
        }
        else if (sprites == ClimbingOnWallSprites)
        {
            eachFrameTime = 0.2f;
        }
        else if (sprites == ActionOnWallSprites)
        {
            eachFrameTime = 0.2f;
        }
        else if (sprites == FallSprites)
        {
            eachFrameTime = 0.2f;
        }
        else if (sprites == HitSprites)
        {
            eachFrameTime = 0.2f;
        }
        else if (sprites == BlockIdleSprites)
        {
            eachFrameTime = 0.2f;
        }
        else if (sprites == BlockHitSprites)
        {
            eachFrameTime = 0.2f;
        }
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public Sprite[] GetCurrentSprites()
    {
        return currentSprites;
    }

    // C2: tạo ra quá nhiều Update có thể khiến game cực nặng và xử lý cũng khó không nice and clean
    // private TimerGeneric timerGeneric;
    // private void Start() // test animation
    // {
    // // ChangeStateAndChangeAnimation(State.Idle);
    // }
    // public void PlayAnFrameAnimation(Sprite[] spriteArr)
    // {
    // spriteRenderer.sprite = spriteArr[idxFrame];
    // idxFrame++;
    // if (idxFrame == spriteArr.Length)
    // {
    // idxFrame = 0;
    // }
    // }
    // public void ChangeStateAndChangeAnimation(State newState) // change state to change animation
    // {
    // if (newState == currentState)
    // {
    // return;
    // }
    // // set state and change others to default
    // currentState = newState;
    // idxFrame = 0;
    // // set animation
    // if (newState == State.Idle)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(IdleSprites);
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.Run)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(RunSprites);
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.Attack1 || newState == State.Attack2 || newState == State.Attack3)
    // {
    // // Random or Order Attack1, 2, 3
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(Attack1Sprites);
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.Die)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(DieSprites);
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.Jump)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(JumpSprites); // Jump Sprite
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.Roll)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(RollSprites); // Jump Sprite
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.ClimbingOnWall)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(ClimbingOnWallSprites); // Jump Sprite
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.ClimbingActionWall)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(ActionOnWallSprites); // Jump Sprite
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.Fall)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(FallSprites); // Jump Sprite
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.Hit)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(HitSprites); // Jump Sprite
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.BlockIdle)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(BlockIdleSprites); // Jump Sprite
    // return true;
    // }, EventArgs.Empty);
    // }
    // if (newState == State.BlockHit)
    // {
    // timerGeneric.SetTimer<EventArgs>(0.2f, true, (object sender, EventArgs eventArgs) =>
    // {
    // PlayAnFrameAnimation(BlockHitSprites); // Jump Sprite
    // return true;
    // }, EventArgs.Empty);
    // }
    // }
}

//   Idle         8 frames 0 - 7 
//   Run        10 frames 
//   Attack1        6 frames 
//   Attack2        6 frames 
//   Attack3        8 frames
//   Block Idle         8 frames
//   Block         5 frames 
//   Jump        3 frames 
//   Fall        4 frames 
//   Roll        4 frames 
//   Ledge Grab       4 frames 
//   Wall Slide       4 frames 
//   Hurt        3 frames 
//   Death        10 frames 
//   Block Effect       10 frames 
//   Slide Dust       10 frames