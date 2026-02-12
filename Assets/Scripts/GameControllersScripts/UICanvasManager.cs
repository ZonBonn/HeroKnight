using UnityEngine;
using UnityEngine.UI;

public class UICanvasManager : MonoBehaviour, IResettable
{
    public static UICanvasManager Instance; // thằng này giữ tham chiếu tới các Bar của UI tới màn nào thì thằng đó Awake tới đây để lấy 

    [SerializeField] PlayerHealthBar playerHealthBar;
    [SerializeField] PlayerStaminaBar playerStaminaBar;
    [SerializeField] BossHealthBar bossHealthBar; 
    
    [SerializeField] GameObject bossHPPannel;
    [SerializeField] GameObject bossHPPannelIntro;
    [SerializeField] BossHealthBarIntro bossHealthBarIntro;

    [SerializeField] CanvasGroup bossHPPannelGroup;
    [SerializeField] CanvasGroup bossHPPannelIntroGroup;

    [SerializeField] GameObject bossHealthBarChild;
    [SerializeField] GameObject bossHealthBarName;
    [SerializeField] GameObject bossHealthBarFrame;

    [SerializeField] GameObject bossHealthBarIntroChild;
    [SerializeField] GameObject bossHealthBarNameIntro;
    [SerializeField] GameObject bossHealthBarFrameIntro;

    // on / off = group
    [SerializeField] CanvasGroup bossFrameGroup;
    [SerializeField] CanvasGroup bossNameGroup;
    [SerializeField] CanvasGroup bossFrameIntroGroup;
    [SerializeField] CanvasGroup bossNameIntroGroup;
    [SerializeField] CanvasGroup bossHealthBGIntroGroup;
    // SetActive
    [SerializeField] GameObject bossHealthBarChildern;
    [SerializeField] GameObject bossHealthBarIntroChildern;

    [SerializeField] GameObject diePannel;
    [SerializeField] GameObject pausedPannel;
    [SerializeField] GameObject intructionPannel;

    public GameObject endingPannel;
    public GameObject instructionKillBossPannel;

    private void Awake()
    {
        // Debug.Log("UICanvasManager Awake from: " + gameObject.scene.name);
        if(Instance == null)
        {
            // Debug.Log("UICanvasManager == null");
            Instance = this;
        }
        else
        {
            // Debug.Log("UICanvasManager != null");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        // Debug.Log("UICanvasManager ID:" + UICanvasManager.Instance.GetHashCode());
    }

    void Start()
    {
        InitStart();
        PlayerHealthSystem.OnTriggerPlayerHealthAsZero += ShowDiePannel;

    }
    private void InitStart()
    {
        // bossHealthBarIntro.OnTriggerFullOfHealthBarIntro += OnFullOfHealthBarIntro; // nếu đăng ký kiểu này thì không được do Object con sẽ được khởi tạo sau mà Obj cha khởi tạo trước nên NRE => giải pháp: tạo một hàm đăng ký cho chính thằng con gọi
        // BossTrigger.OnTriggerBossStartFighting += OnBossStartFighting;
    }   

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIButtonAction.Instance.PausedButton();
        }
    }

    void OnDisable()
    {
        HidePannel(endingPannel);
        HidePannel(diePannel);
    }

    // cách đăng ký hay khi không biết được thứ tự sinh ra: + 1 kinh nghiệm
    // B1: gọi hàm từ chỗ "lẽ ra cần đăng ký tại đó
    // B2: truyền chính nó cho hàm xử lý đăng ký
    // B3: cho nó tự gọi tự đăng ký
    public void RegisterBossIntroBar(BossHealthBarIntro intro) // hàm này được gọi khi cái class chứa delegate Action được sinh ra
    {
        bossHealthBarIntro = intro;
        bossHealthBarIntro.OnTriggerFullOfHealthBarIntro += OnFullOfHealthBarIntro;
    }
    public void RegisterBossStartFighting() // được đăng ký khi cái hàm nào đó được khởi tạo và gọi hàm này thì sẽ tự đăng ký
    {
        BossTrigger.OnTriggerBossStartFighting += OnBossStartFighting;
    }

    // ======================== SUBCRIBERS ========================
    public void OnBossStartFighting() // cái cách đăng ký này áp dụng cho obj con của canvas vì nó bị khởi tạo sau, nên sẽ đăng ký sau khi nó được khởi tạo
    {
        shoudShowBossHPIntroPannel(true);
        BossTrigger.OnTriggerBossStartFighting -= OnBossStartFighting;
    }

    public void OnFullOfHealthBarIntro()
    {
        shoudShowBossHPIntroPannel(false);
        shoudShowBossHPPannel(true);
        bossHealthBarIntro.OnTriggerFullOfHealthBarIntro -= OnFullOfHealthBarIntro; // chạy xong thì hủy đăng ký
    }
    // ============================================================

    // làm thế này rebuild cho nó không bị giựt lag
    public void shoudShowBossHPPannel(bool shouldShow)
    {
        if(shouldShow == true)
        {
            bossFrameGroup.alpha = 1; bossFrameGroup.interactable = true; bossFrameGroup.blocksRaycasts = true;
            bossNameGroup.alpha = 1; bossNameGroup.interactable = true; bossNameGroup.blocksRaycasts = true;
            bossHealthBarChildern.SetActive(true);
        }
        else
        {
            bossFrameGroup.alpha = 0; bossFrameGroup.interactable = false; bossFrameGroup.blocksRaycasts = false;
            bossNameGroup.alpha = 0; bossNameGroup.interactable = false; bossNameGroup.blocksRaycasts = false;
            bossHealthBarChildern.SetActive(false);
        }
    }

    private void shoudShowBossHPIntroPannel(bool shouldShow)
    {
        if(shouldShow == true)
        {
            bossFrameIntroGroup.alpha = 1; bossFrameIntroGroup.interactable = true; bossFrameIntroGroup.blocksRaycasts = true;
            bossNameIntroGroup.alpha = 1; bossNameIntroGroup.interactable = true; bossNameIntroGroup.blocksRaycasts = true;
            bossHealthBGIntroGroup.alpha = 1; bossHealthBGIntroGroup.interactable = true; bossHealthBGIntroGroup.blocksRaycasts = true;
            bossHealthBarIntroChildern.SetActive(true);
        }
        else
        {
            bossFrameIntroGroup.alpha = 0; bossFrameIntroGroup.interactable = false; bossFrameIntroGroup.blocksRaycasts = false;
            bossNameIntroGroup.alpha = 0; bossNameIntroGroup.interactable = false; bossNameIntroGroup.blocksRaycasts = false;
            bossHealthBGIntroGroup.alpha = 0; bossHealthBGIntroGroup.interactable = false; bossHealthBGIntroGroup.blocksRaycasts = false;
            bossHealthBarIntroChildern.SetActive(false);
        }
    }
     
    public PlayerHealthBar getPlayerHealthBar(){ return playerHealthBar; }
    public PlayerStaminaBar getPlayerStaminaBar() { return playerStaminaBar; }
    public BossHealthBar getBossHealthBar(){ return bossHealthBar; }
    public BossHealthBarIntro getBossHealthBarIntro(){ return bossHealthBarIntroChildern.GetComponent<BossHealthBarIntro>(); }

    public void ShowPannel(GameObject pannel)
    {
        pannel.SetActive(true);
    }

    public void HidePannel(GameObject pannel)
    {
        pannel.SetActive(false);
    }

    public void ResetState()
    {
        shoudShowBossHPIntroPannel(false);
        shoudShowBossHPPannel(false);
        bossHealthBarIntroChildern.GetComponent<BossHealthBarIntro>().Reset();
    }

    private void ShowDiePannel()
    {
        ShowPannel(diePannel);
    }

}
