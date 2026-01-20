using UnityEngine;

public class BossPositionHolder : MonoBehaviour
{
    public static BossPositionHolder Instance;

    private Transform realBossTransform; // xử lý vị trí riêng vì sprite nó bị lệch không dùng transform của GameObject cha được mà phải dùng một GameObject con để làm tâm

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        realBossTransform = gameObject.transform.Find("BossRealPosition");
    }

    public Vector3 GetRealBossPosition()
    {
        return realBossTransform.position;
    }
}
