
using System;
using UnityEngine;

public struct DamageInfo // trao đổi thông tin qua cái này, người sender gửi thông số cho nó tự xử lý hoặc người xử lý tự xử lý cho người nhận chỉ việc nhận
{
    public float baseDamage; //
    public float minDamage; // damage từ enemy gửi tới player 
    public float maxDamage; // damage từ enemy gửi tới player 
    public int attackerDir; // hướng từ enemy gửi tới player 

    public AttackType attackType;

    public LayerMask layerMask;
}