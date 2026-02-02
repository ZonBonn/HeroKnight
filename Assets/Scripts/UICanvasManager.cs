using UnityEngine;

public class UICanvasManager : MonoBehaviour
{
    public static UICanvasManager Instance; // thằng này giữ tham chiếu tới các Bar của UI tới màn nào thì thằng đó Awake tới đây để lấy 

    [SerializeField] PlayerHealthBar playerHealthBar;
    [SerializeField] PlayerStaminaBar playerStaminaBar;
    [SerializeField] BossHealthBar bossHealthBar; 
    
    [SerializeField] GameObject bossHPPannel;
    [SerializeField] GameObject bossHPPannelIntro;
    [SerializeField] BossHealthBarIntro bossHealthBarIntro;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
        
    }

    void Start()
    {
        InitStart();

    }
    private void InitStart()
    {
        // bossHealthBarIntro.OnTriggerFullOfHealthBarIntro += OnFullOfHealthBarIntro; // nếu đăng ký kiểu này thì không được do Object con sẽ được khởi tạo sau mà Obj cha khởi tạo trước nên NRE => giải pháp: tạo một hàm đăng ký cho chính thằng con gọi
        BossTrigger.OnTriggerBossStartFighting += OnBossStartFighting;
    }
    
    void Update()
    {
        
    }
    
    public PlayerHealthBar getPlayerHealthBar(){ return playerHealthBar; }
    public PlayerStaminaBar getPlayerStaminaBar() { return playerStaminaBar; }
    public BossHealthBar getBossHealthBar(){ return bossHealthBar; }

    public void OnFullOfHealthBarIntro()
    {
        bossHPPannelIntro.SetActive(false);
        bossHPPannel.SetActive(true);
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

    public void OnBossStartFighting()
    {
        bossHPPannelIntro.SetActive(true);
        bossHPPannel.SetActive(true);
    }
}
