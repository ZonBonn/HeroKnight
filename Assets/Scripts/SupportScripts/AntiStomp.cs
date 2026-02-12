using UnityEngine;

public class AntiStomp : MonoBehaviour
{
    [SerializeField] LayerMask playerLayerMask;
    private Collider2D myCollider;
    
    private void Start()
    {
        myCollider = gameObject.GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        // Debug.Log("đang va chạm với Anti Stomp");
        if(!collider2D.gameObject.CompareTag("Player")) return;

        FunctionTimer.Create(CheckAfterAmountOfTime, 0.5f);
    }

    private void CheckAfterAmountOfTime() // kiểm tra sau 1 giây liệu còn va chạm và vận tốc y còn là 0 không ? => player đang bị kẹt
    {
        if (myCollider.IsTouchingLayers(playerLayerMask))
        {
            Collider2D[] contacts2D = new Collider2D[5];
            int count = myCollider.GetContacts(contacts2D);

            for(int i = 0 ; i < count ; i++)
            {
                if (contacts2D[i].CompareTag("Player"))
                {
                    Rigidbody2D rb2D = contacts2D[i].gameObject.GetComponent<Rigidbody2D>();
                    if(rb2D == null) return;

                    // C1: check thêm cả vận tốc 0 cho chắc chắn người chơi đang bị kẹt (hoặc đẩy số 0 lớn hơn một tí cho chắc chắn)
                    if(rb2D.linearVelocityY <= 0)
                    {
                        if(transform.parent != null)
                        {
                            // Debug.Log("người chơi bị kẹt, chuyển layer into StompEnemy chống kẹt cho player");
                            gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer("StompEnemy");
                            FunctionTimer.Create(ChangeIntoEnemyLayer, 0.2f);
                        }
                    }

                    // C2: không cần check liệu LinearVelocityY = 0 ? => đôi khi không == 0 thì kẹt vĩnh viễn à ?
                }
            }

            
        }
    }

    private void ChangeIntoEnemyLayer()
    {
        gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

}
