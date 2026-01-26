using UnityEngine;
using System;

public class BossSkill1 : MonoBehaviour
{
    private float timeRemainSkill = 10f; // thời gian duy trì skill
    private float m_timeRemainSkill; // đếm ngược duy trì thời gian duy trì skill

    private bool CanKeepUseSkill1; // liệu có thể tiếp tục tàng hình không 

    public Action OnTriggerEndOfVisible;

    private bool CanUseSkill1; // liệu có thể dùng skill 1 không
    private float timer; // thời gian để hồi skill 1
    private float coolDownSkill1 = 15f; // đến ngược thời gian hồi skill 1

    private void Start()
    {
        timer = coolDownSkill1;
    }

    private void Update()
    {
        if(m_timeRemainSkill > 0) // thời gian duy trì tàng hình
        {
            CanKeepUseSkill1 = true;
            m_timeRemainSkill -= Time.deltaTime;
            if(m_timeRemainSkill <= 0)
            {
                OnTriggerEndOfVisible?.Invoke(); // hết tàng hình
                CanKeepUseSkill1 = false;
                timer = coolDownSkill1; // khi thời gian duy trì skill không còn nữa => hết skill bắt đầu đếm ngược thời gian hồi skill 1
                return;
            }
        }

        if(timer > 0) // thời gian đếm ngược hồi skill tàng hình
        {
            CanUseSkill1 = false;
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                CanUseSkill1 = true;
            }
            
        }
    }

    public void SetDefaultValueForSkill1()
    {
        m_timeRemainSkill = timeRemainSkill;
        CanKeepUseSkill1 = true;
    }

    public bool getCanKeepUseSkill1()
    {
        return CanKeepUseSkill1;
    }



    public void SetSkill1CoolDown()
    {
        timer = coolDownSkill1;
    }

    public bool getCanUseSkill1()
    {
        return CanUseSkill1;
    }

    public void UseSkill1() // không giống như skill 2 là gọi cái thì bắt đầu đếm ngược luôn (thì cái UseSkill 2 đặt luôn là false), còn cái này thì phải hết tàng hình thì mới bắt đầu đếm ngược thì mới false được
    {
        CanUseSkill1 = false;
    }
}
