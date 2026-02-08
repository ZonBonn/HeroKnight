using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    // HÀM CHẠY: tính vị trí chuột được chuẩn hóa theo kích thước màn hình và di chuyển tới nó
    public float moveSpeed = 0.25f; // kiểm soát tốc độ di chuển
    public float smoothTime = .3f; // Sau đúng smoothTime -> object tới đích
    private Vector3 startPosition;
    private Vector3 velocity;

    private void Start()
    {
        startPosition = transform.position; // vị trí đầu tiên để gameObject bám vào rồi chỉ di chuyển xung quanh cái đó
    }

    private void Update()
    {
        Vector3 nextPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition); // chuyển vị trí của chuột thành vị trí normolize theo màn hình
        transform.position = Vector3.SmoothDamp(transform.position, startPosition - (nextPosition * moveSpeed), ref velocity, smoothTime);
    }
}