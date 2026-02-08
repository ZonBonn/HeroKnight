using System;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private Transform symbolTransform;
    
    public static Action OnTriggerStoneSymbolActive; // static (trông cho nó sạch đỡ phải tham chiếu biến ở BossAI trông bẩn) vì chỉ có một cục đá này ở màn boss thôi, nhưng nếu có nhiều cục đá thì gửi về cái ID của stone để check thông qua Action<Stone> StoneSignals
    public static Action OnTriggerStoneSymbolOff;

    private BossHealthBarIntro bossHealthBarIntro;

    private void Awake()
    {
        symbolTransform = gameObject.transform.Find("StoneSymbol");
    }

    private void Start()
    {
        bossHealthBarIntro = UICanvasManager.Instance.getBossHealthBarIntro();
        bossHealthBarIntro.OnTriggerFullOfHealthBarIntro += StoneSymbolOn;
        BossAI.OnTriggerBossDeath += StoneSymbolOff;
    }

    public void shouldActiveStoneSymbol(bool shouldActive)
    {
        symbolTransform.gameObject.SetActive(shouldActive);
        if(shouldActive == true)
        {
            FunctionTimer.Create(OnTriggerStoneSymbolActive, 0.5f);
        }
    }

    public void StoneSymbolOn()
    {
       symbolTransform.gameObject.SetActive(true);
       FunctionTimer.Create(OnTriggerStoneSymbolActive, 0.5f);
    }

    public void StoneSymbolOff()
    {
        symbolTransform.gameObject.SetActive(false);
    }
}
