using UnityEngine;

public class EnemyEWShooting : MonoBehaviour
{
    private EnemyEWAnimation enemyEWAnimation;
    private EnemyEWPathFindingMovement enemyEWPathFindingMovement; 

    public GameObject bullet;

    private Transform firePositionLeft;
    private Transform firePositionRight;

    private void Awake()
    {
        enemyEWAnimation = gameObject.GetComponent<EnemyEWAnimation>();
        enemyEWPathFindingMovement = gameObject.GetComponent<EnemyEWPathFindingMovement>();

        firePositionLeft = gameObject.transform.Find("FirePositionLeft");
        firePositionRight = gameObject.transform.Find("FirePositionRight");
    }

    private void Start()
    {
        enemyEWAnimation.OnTriggerEachFrames += CreateAttack;
    }

    public void CreateAttack(int idxFrame, Sprite[] sprites)
    {
        if(sprites == enemyEWAnimation.AttackSprites && idxFrame == 10)
        {
            Vector3 firePos;

            int visualDir = enemyEWPathFindingMovement.currentVisualDir;
            
            firePos = visualDir == +1 ? firePositionRight.position : firePositionLeft.position;
            
            Vector3 playerPosition = Player.Instance.GetPlayerPosition();
            Vector3 dir = (playerPosition - firePos).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            GameObject bulletObject = Instantiate(bullet, firePos, Quaternion.Euler(0f, 0f, angle));
            ProjectileMoving projectileMoving = bulletObject.GetComponent<ProjectileMoving>();
            ProjectileCollision projectileCollision = bulletObject.GetComponent<ProjectileCollision>();
            projectileCollision.SetShooter(this.gameObject);
            
            projectileMoving.Fire(dir);
        }
    }
}
