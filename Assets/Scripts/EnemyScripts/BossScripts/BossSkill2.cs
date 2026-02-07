using UnityEngine;
using System;

public class BossSkill2 : MonoBehaviour
{
    private BossSkill2Animation bossSkill2Animation;
    private const float ATTACK_DISTANCE = 1.5f;
    private const int MIN_DAMAGE_SKILL_2 = 50;
    private const int MAX_DAMAGE_SKILL_2 = 50; 
    public LayerMask playerLayerMask;

    private void Awake()
    {
        bossSkill2Animation = gameObject.GetComponent<BossSkill2Animation>();
    }

    private void Start()
    {
        bossSkill2Animation.OnTriggerEachFrames += OnTriggerCreateAttackPoint;
        bossSkill2Animation.OnTriggerLastFrames += OnTriggerLastSkill2Frame;
    }

    private void Update()
    {
        
    }

    private void OnTriggerCreateAttackPoint(int idxFrame, Sprite[] sprites)
    {
        if(sprites == bossSkill2Animation.Skill2Sprite && idxFrame == 8)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(gameObject.transform.position, Vector2.down, 1.5f, playerLayerMask);

            // GameObject playerGameObject;
            // C1: non interface
            // if(raycastHit2D.collider != null)
            // {
            //     playerGameObject = raycastHit2D.collider.gameObject;
            //     if(playerGameObject.CompareTag("Player"))
            //     {
            //         PlayerHealthStaminaHandler playerHealthStaminaHandler = playerGameObject.GetComponent<PlayerHealthStaminaHandler>();
            //         if(playerHealthStaminaHandler != null)
            //         {
            //             // playerHealthStaminaHandler.DamageHealth(UnityEngine.Random.Range(MIN_DAMAGE, MAX_DAMAGE));
            //         }
            //     }
            // }
            // C2: interface
            if(raycastHit2D.collider != null)
            {
                IDamageable damageable = raycastHit2D.collider.gameObject.GetComponent<IDamageable>();
                if(damageable != null)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.attackerDir = 0;
                    damageInfo.minDamage = MIN_DAMAGE_SKILL_2;
                    damageInfo.maxDamage = MAX_DAMAGE_SKILL_2;
                    damageInfo.layerMask = gameObject.layer; // cái vòng triệu hồi này cũng nên cho là Layer Enemy dù nó chỉ là skill được gọi :D
                    damageable.Damage(damageInfo);
                }
            }
        }
    }

    private void OnTriggerLastSkill2Frame(Sprite[] sprites)
    {
        // về sau set cooldown skill2 tại đây 


        
        if(sprites == bossSkill2Animation.Skill2Sprite)
            Destroy(gameObject);
    }
}
