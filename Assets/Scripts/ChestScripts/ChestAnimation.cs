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
    private ChestItemsHolder chestItemsHolder;

    private SpriteRenderer spriteRenderer;

    public Action<int, Sprite[]> OnTriggerEachChestFrame;
    public Action OnTriggerAfterDoneLastFrameChest;

    private bool CanKeepOpenForFirstTime;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        chest = gameObject.GetComponent<Chest>();
        chestItemsHolder = gameObject.GetComponent<ChestItemsHolder>();

        chestType = chest.GetChestType();

        WhichChestTypeForCurrentSprite(chestType);
    } 
    
    private void Start()
    {   
        idxChestFrame = 0;

        TimerCoolDown = TIME_EACH_FRAME_CHEST;

        OnTriggerAfterDoneLastFrameChest += OnTriggerFirstTimeOpenChest;

        CanKeepOpenForFirstTime = false;
    }

    private void Update()
    {
        bool IsPlayerNearVar = chest.getIsPlayerNear();
        bool IsOpendedVar = chest.getIsOpended();
        bool IsUsedToOpenVar = chest.getIsUsedToOpen();
        bool IsFristTimeOpen = chest.getIsFristTimeOpen();
        bool IsSpawnedAllItems = chestItemsHolder.getSpawnedAllItems();

        // mở khi: người chơi gần VÀ được mở HOẶC đã từng được mở, nếu mở lần đầu thì nó sẽ không tự đóng lại nếu người chơi đi xa, còn từ lần 2 trở đi thì ok cứ đóng thoải mái
        if((IsPlayerNearVar == true || CanKeepOpenForFirstTime == true) && (IsOpendedVar == true || IsUsedToOpenVar == true)) 
        {
            Debug.Log("Open");
            ChangeCurrentChestSprites(currentChestOpenSprites);
            PlayChestAnimation();
        }
        // đóng khi: người chơi xa VÀ chưa từng được mở HOẶC từng mở rồi thì vẫn có thể đóng, hoặc chỉ khi nào spawn xong đồ thì mới cho đóng
        else if(IsPlayerNearVar == false && (IsOpendedVar == false || IsUsedToOpenVar == true) && CanKeepOpenForFirstTime == false && IsSpawnedAllItems == true/* && chest.getIsFristTimeOpen() == true*/) 
        {
            if(IsFristTimeOpen == true)
            {
                Debug.Log("Close");
                ChangeCurrentChestSprites(currentChestCloseSprites);
                PlayChestAnimation();
            }
            
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
            Debug.Log("Chest frame IDX: " + idxChestFrame);
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
                Debug.Log("frame cuối của chest");
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
        if(chest.getIsFristTimeOpen() == true)
        {
            caculateNewIdxChestFrame();
        }
        
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

    // ================= FUNCTION FOR PUBLISHER SIGN =================
    private void OnTriggerFirstTimeOpenChest()
    {
        if(chest.getIsFristTimeOpen() == false)
        {
            chest.setIsFristTimeOpen(true);
            CanKeepOpenForFirstTime = false; // thằng này được thay đổi bởi Delegate của ChestItemsHolder
            OnTriggerAfterDoneLastFrameChest -= OnTriggerFirstTimeOpenChest; // xong rồi thì hủy đăng ký thôi
        }
    }

    // ===============================================================



    // ==================== FUNCTION SUPPORTING ======================

    public void SetCanKeepOpenForFirstTime(bool shouldKeepOpen)
    {
        CanKeepOpenForFirstTime = shouldKeepOpen;
    }

    public void SetCanKeepOpenForFirstTimeTrue()
    {
        CanKeepOpenForFirstTime = true;
    }

    public void SetCanKeepOpenForFirstTimeFalse()
    {
        CanKeepOpenForFirstTime = false;
    }
    // ===============================================================    
}
