using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    static int layerSortOrder;
    private TextMeshPro textMeshPro;

    [SerializeField] float moveSpeed;
    [SerializeField] float disappearTimer; // thời gian tồn tại
    private float DISAPPEAR_TIMER_MAX;
    Color tmpTextMeshProColor;

    // Create damage popup
    public static DamagePopup Create(Vector3 pos, float damageAmount, bool isCriticalHit)
    {
        Transform pfDamagePopup = Instantiate(GameAssets.i.pfDamagePopup, pos, Quaternion.identity); // một đối tượng bên ngoài chỉ việc gọi hàm này và DamagePopup tự tạo ra DamagePopup
        DamagePopup damagePopup = pfDamagePopup.GetComponent<DamagePopup>();
        damagePopup.SetUp(damageAmount, isCriticalHit);
        return damagePopup;
    }

    private void Awake()
    {
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
        
    }

    private void SetUp(float damageAmount, bool isCriticalHit)
    {
        if(isCriticalHit == true)
        {
            textMeshPro.fontSize = 6f;
            textMeshPro.color = Color.red;
        }
        else
        {
            textMeshPro.fontSize = 4f;
            textMeshPro.color = Color.yellow;
        }

        textMeshPro.SetText(damageAmount.ToString());
        tmpTextMeshProColor = textMeshPro.color;
        DISAPPEAR_TIMER_MAX = disappearTimer;

        textMeshPro.sortingOrder = layerSortOrder++;
    }

    private void Update()
    {
        // effect
        Vector3 moveVector = Vector3.one * moveSpeed;
        gameObject.transform.position += moveVector * Time.deltaTime;
        float slowDownSpeed = 2f;
        moveVector -= moveVector * slowDownSpeed * Time.deltaTime; // giảm tốc độ dần dần


        if(disappearTimer >= DISAPPEAR_TIMER_MAX * .5f) // nửa đầu
        {
            float increaseScaleAmount = 1f;
            textMeshPro.fontSize += increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float decreaseScaleAmount = 1f;
            textMeshPro.fontSize -= decreaseScaleAmount * Time.deltaTime;
        }

        // disappear
        disappearTimer -= Time.deltaTime;
        if(disappearTimer <= 0)
        {
            float disappearSpeed = 5f; // tốc độ biến mất
            tmpTextMeshProColor.a -= disappearSpeed * Time.deltaTime;
            textMeshPro.color = tmpTextMeshProColor;
            if (textMeshPro.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
