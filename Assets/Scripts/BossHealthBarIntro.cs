using UnityEngine;
using UnityEngine.UI;
using System;

public class BossHealthBarIntro : MonoBehaviour
{
    public Action OnTriggerFullOfHealthBarIntro;
    private float fillSpeed = 0.3f; // mất 1/3 giây để đầy <=> fillAmount [0->1] => 1/0.5 = 2


    private Image HealthBarIntro;

    private void Awake()
    {
        HealthBarIntro = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        UICanvasManager.Instance.RegisterBossIntroBar(this); // đăng ký hàm khi mà nó được sinh ra
    }
    
    private void Update()
    {
        RegenHP();
    }
    
    private void RegenHP()
    {
        if(HealthBarIntro.fillAmount >= 1)
        {
            HealthBarIntro.fillAmount = 1f;
            OnTriggerFullOfHealthBarIntro?.Invoke();
            return;
        }
        HealthBarIntro.fillAmount += fillSpeed * Time.deltaTime;
    }
}
