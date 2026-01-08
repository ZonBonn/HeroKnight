using UnityEngine;
using System;

public class ChestAnimation : MonoBehaviour
{
    public Sprite[] WoodenChestOpenSprites;
    public Sprite[] WoodenChestCloseSprites;
    
    public Sprite[] IronChestOpenSprites;
    public Sprite[] IronChestCloseSprites;
    
    public Sprite[] SilverChestOpenSprites;
    public Sprite[] SilverChestCloseSprites;

    public Sprite[] GoldenChestOpenSprites;
    public Sprite[] GoldenChestCloseSprites;

    private Sprite[] currentChestSprites;
    private Sprite[] currentChestOpenSprites;
    private Sprite[] currentChestCloseSprites;

    private bool isLoop = false;
    private int idxChestFrame;
    private float TimerCoolDown;
    private const float TIME_EACH_FRAME_CHEST = 0.05f;
    private const int DEFAULT_CHEST_FRAMES = 7;

    private Chest.ChestType chestType;
    private Chest chest;

    private SpriteRenderer spriteRenderer;

    public Action<int, Sprite[]> OnTriggerEachChestFrame;
    public Action OnTriggerAfterDoneLastFrameChest;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        chest = gameObject.GetComponent<Chest>();
        chestType = chest.GetChestType();

        WhichChestTypeForCurrentSprite(chestType);
    } 
    
    private void Start()
    {   
        idxChestFrame = 0;

        TimerCoolDown = TIME_EACH_FRAME_CHEST;
    }

    private void Update()
    {
        bool IsPlayerNearVar = chest.getIsPlayerNear();
        bool IsOpendedVar = chest.getIsOpended();
        if(IsPlayerNearVar == true && IsOpendedVar == true) // người chơi gần
        {
            ChangeCurrentChestSprites(currentChestOpenSprites);
            PlayChestAnimation();
        }

        if(IsPlayerNearVar == false && IsOpendedVar == false) // người chơi không gần
        {
            ChangeCurrentChestSprites(currentChestCloseSprites);
            PlayChestAnimation();
        }
        
    }

    private void PlayChestAnimation()
    {
        TimerCoolDown -= Time.deltaTime;
        if(TimerCoolDown <= 0)
        {
            OnTriggerEachChestFrame?.Invoke(idxChestFrame, currentChestSprites);
            spriteRenderer.sprite = currentChestSprites[idxChestFrame];
            ++idxChestFrame;
            if(idxChestFrame == currentChestSprites.Length)
            {
                if(isLoop == true)
                {
                    idxChestFrame = 0;
                }
                else if(isLoop == false)
                {
                    idxChestFrame--;
                }
                OnTriggerAfterDoneLastFrameChest?.Invoke();
            }
            TimerCoolDown = TIME_EACH_FRAME_CHEST;
        }
    }

    private void PlayChestOpenAnimation()
    {
        TimerCoolDown -= Time.deltaTime;
        if(TimerCoolDown <= 0)
        {
            spriteRenderer.sprite = currentChestOpenSprites[idxChestFrame];
            ++idxChestFrame;
            if(idxChestFrame == currentChestOpenSprites.Length)
            {
                if(isLoop == true)
                {
                    idxChestFrame = 0;
                }
                else if(isLoop == false)
                {
                    idxChestFrame--;
                }
                
            }
            TimerCoolDown = TIME_EACH_FRAME_CHEST;
        }
    }

    private void PlayChestCLoseAnimation()
    {
        TimerCoolDown -= Time.deltaTime;
        if(TimerCoolDown <= 0)
        {
            spriteRenderer.sprite = currentChestCloseSprites[idxChestFrame];
            ++idxChestFrame;
            if(idxChestFrame == currentChestCloseSprites.Length)
            {
                if(isLoop == true)
                {
                    idxChestFrame = 0;
                }
                else if(isLoop == false)
                {
                    idxChestFrame--;
                }
                
            }
            TimerCoolDown = TIME_EACH_FRAME_CHEST;
        }
    }

    private void ChangeCurrentChestSprites(Sprite[] newCurrentChestSprites)
    {
        if(newCurrentChestSprites == currentChestSprites) return;
        currentChestSprites = newCurrentChestSprites;
        caculateNewIdxChestFrame();
        ResetTimerCoolDown();
    }

    private void caculateNewIdxChestFrame() // mặc định đóng và mở có CỐ ĐỊNH 7 frames
    {
        
        idxChestFrame = DEFAULT_CHEST_FRAMES - idxChestFrame - 1;
        // return idxChestFrame;
    }

    private void ResetTimerCoolDown()
    {
        Debug.Log("Reseted Timer");
        TimerCoolDown = TIME_EACH_FRAME_CHEST;
    }

    private void WhichChestTypeForCurrentSprite(Chest.ChestType chestType)
    {
        if (chestType == Chest.ChestType.Golden)
        {
            currentChestOpenSprites = GoldenChestOpenSprites;
            currentChestCloseSprites = GoldenChestCloseSprites;
        }
        else if (chestType == Chest.ChestType.Wooden)
        {
            currentChestOpenSprites = WoodenChestOpenSprites;
            currentChestCloseSprites = WoodenChestCloseSprites;
        }
        else if (chestType == Chest.ChestType.Iron)
        {
            currentChestOpenSprites = IronChestOpenSprites;
            currentChestCloseSprites = IronChestCloseSprites;
        }
        else if (chestType == Chest.ChestType.Silver)
        {
            currentChestOpenSprites = SilverChestOpenSprites;
            currentChestCloseSprites = SilverChestCloseSprites;
        }
    }
}
