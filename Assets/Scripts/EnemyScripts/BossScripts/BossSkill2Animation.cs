using UnityEngine;
using System;

public class BossSkill2Animation : MonoBehaviour
{
    public Sprite[] Skill2Sprite;
    private int idxSkillFrames;
    private float timerChangeIdxSkillFrames;
    private float m_timerChangeIdxSkillFrame;
    private bool isLoop;

    public Action<int, Sprite[]> OnTriggerEachFrames;
    public Action<Sprite[]> OnTriggerLastFrames;

    private SpriteRenderer spriteRenderer;

    private void InitAwake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        idxSkillFrames = 0;
        timerChangeIdxSkillFrames = 0.1f;
        m_timerChangeIdxSkillFrame = timerChangeIdxSkillFrames;
    }

    private void InitStart()
    {
        
    }

    private void Awake()
    {
        InitAwake();
    }

    private void Start()
    {
        InitStart();
    }

    private void Update()
    {
        PlayAnimation();
    }

    private void PlayAnimation() // gọi cái này trong FunctionTimer.Create() cũng được
    {
        m_timerChangeIdxSkillFrame -= Time.deltaTime;
        if(m_timerChangeIdxSkillFrame <= 0)
        {
            spriteRenderer.sprite = Skill2Sprite[idxSkillFrames++];
            m_timerChangeIdxSkillFrame = timerChangeIdxSkillFrames;

            OnTriggerEachFrames?.Invoke(idxSkillFrames, Skill2Sprite);
        }
        if (idxSkillFrames == Skill2Sprite.Length) // đoạn này thực hiện khi chạy m_timerChangeIdxBodyFrameBodyFrame của frame cuối cùng rồi
        {
            if(isLoop == true)
            {
                idxSkillFrames = 0;
            }
            else
            {
                idxSkillFrames = Skill2Sprite.Length-1; // stop at the last frame if isLoop == false
            }

            OnTriggerLastFrames?.Invoke(Skill2Sprite);
        }
    }
}
