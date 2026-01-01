using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    public float ClampXLeft;
    public float ClampXRight;
    public float ClampYUp;
    public float ClampYDown;

    public static LevelBoundary Instance;

    private void Awake()
    {
        Instance = this; // Mỗi scene load sẽ cập nhật Instance mới
    }
}
