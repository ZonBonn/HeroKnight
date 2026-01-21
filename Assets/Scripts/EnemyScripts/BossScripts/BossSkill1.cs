using UnityEngine;
using System;

public class BossSkill1 : MonoBehaviour
{
    private float timeRemainSkill = 10f;
    private float m_timeRemainSkill;

    private bool CanKeepUseSkill1;

    public Action OnTriggerEndOfVisible;

    private void Update()
    {
        if(m_timeRemainSkill > 0)
        {
            CanKeepUseSkill1 = true;
            m_timeRemainSkill -= Time.deltaTime;
            if(m_timeRemainSkill <= 0)
            {
                OnTriggerEndOfVisible?.Invoke(); // hết tàng hình
                CanKeepUseSkill1 = false;
                return;
            }
        }
    }

    public void SetDefaultValueForSkill1()
    {
        m_timeRemainSkill = timeRemainSkill;
    }

    public bool getCanKeepUseSkill1()
    {
        return CanKeepUseSkill1;
    }
}
