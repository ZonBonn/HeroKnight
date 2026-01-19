using UnityEngine;
using UnityEngine.Rendering;
using System;

public enum ProjectileState
{
    Moving,
    Explode
}

public class ProjectileAnimation : MonoBehaviour
{
    public Sprite[] projectileMoveSprites;
    public Sprite[] projectileExplodeSprites;

    private Sprite[] currentSprite;

    private int idxProjectileIdx;
    private float timerChangeProjectileIdx = 0.2f;
    private float m_timerChangeProjectileIdx;

    private bool isLoop;

    private SpriteRenderer spriteRenderer;

    public Action<Sprite[], int> OnTriggerEachFrame;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        m_timerChangeProjectileIdx = timerChangeProjectileIdx;
        ProjectileAnimationHandler(ProjectileState.Moving);
        
        OnTriggerEachFrame += OnLastOfExpldeFrame;
    }

    private void Update()
    {
        PlayProjectileAnimation();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ProjectileAnimationHandler(ProjectileState.Moving);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ProjectileAnimationHandler(ProjectileState.Explode);
        }
    }

    private void PlayProjectileAnimation()
    {
        m_timerChangeProjectileIdx -= Time.deltaTime;   
        if(m_timerChangeProjectileIdx <= 0)
        {
            OnTriggerEachFrame?.Invoke(currentSprite, idxProjectileIdx); // ?? không return được frame cuói -> continue work in here <--

            spriteRenderer.sprite = currentSprite[idxProjectileIdx];
            ++idxProjectileIdx;
            if(idxProjectileIdx == currentSprite.Length)
            {
                if(isLoop == true)
                {
                    idxProjectileIdx = 0;
                    // m_timerChangeProjectileIdx = timerChangeProjectileIdx;
                }
                else if(isLoop == false)
                {
                    idxProjectileIdx = currentSprite.Length-1;
                }
            }
            m_timerChangeProjectileIdx = timerChangeProjectileIdx;
        }
    }

    public void ProjectileAnimationHandler(ProjectileState state)
    {
        if(state == ProjectileState.Moving)
        {
            ChangeProjectileAnimationByState(state);
        }
        else if(state == ProjectileState.Explode)
        {
            ChangeProjectileAnimationByState(state);
        }
        else
        {
            Debug.Log("chưa có state này thêm ở ProjectileAnimationHandler");
        }
    }

    private void ChangeProjectileAnimationByState(ProjectileState state)
    {
        if(state == ProjectileState.Moving)
        {
            ChangeAnimation(projectileMoveSprites, true);
        }
        else if(state == ProjectileState.Explode)
        {
            ChangeAnimation(projectileExplodeSprites, false);
        }
        else
        {
            Debug.Log("chưa có state này thêm ở ChangeProjectileAnimationByState");
        }
    }

    private void ChangeAnimation(Sprite[] newAnimation, bool isLoop)
    {
        if(newAnimation == currentSprite) return;

        // set up for new animation
        currentSprite = newAnimation;
        this.isLoop = isLoop;
        idxProjectileIdx = 0;
        m_timerChangeProjectileIdx = timerChangeProjectileIdx;
        changeTimerChangeProjectileIdx(newAnimation);
    }

    private void changeTimerChangeProjectileIdx(Sprite[] newAnimation)
    {
        if(newAnimation == projectileExplodeSprites)
        {
            timerChangeProjectileIdx = 0.08f;
        }
        else if(newAnimation == projectileMoveSprites)
        {
            timerChangeProjectileIdx = 0.08f;
        }
    }

    // ================== TRIGGER FRAMES ==================
    private void OnLastOfExpldeFrame(Sprite[] sprites, int idxFrame)
    {
        if(sprites == projectileExplodeSprites && idxFrame == sprites.Length)
        {
            Destroy(gameObject);
        }
    }
}
