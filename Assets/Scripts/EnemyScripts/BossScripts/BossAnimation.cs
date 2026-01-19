using UnityEngine;


public enum BossState
{
    Idle,
    Walk,
    Attack,
    Death,
    Hurt,
    Skill1, // visible
    Skill2, // far attack distance
    PrepareSkill2,
    Unvisible

}
public class BossAnimation : MonoBehaviour
{
    public Sprite[] IdleSprites; // 0-7
    public Sprite[] WalkSprites; // 8-15
    public Sprite[] AttackSprites; // 16 -25
    public Sprite[] DeathSprites; // 29 - 38
    public Sprite[] HurtSprites; // 26-28
    public Sprite[] Skill1Sprites; // 29 - 38
    public Sprite[] Skill2Sprites; // 48 - 63 cái này sẽ dành cho skill riêng
    public Sprite[] PrepareSkill2Sprites; // 39 - 47
    public Sprite[] UnvisibleSprites; // 38 - 29
}
