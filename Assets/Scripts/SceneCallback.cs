using UnityEngine;

public class SceneCallback : MonoBehaviour
{
    // EXPLAIN:
    // màn hình chỉ cập nhật khi có Update nào đó được được
    // nếu hai dòng mã load scene mà cùng nằm trên một dòng thì sẽ không load được scene đầu tiên mà khi load xong scene ở dòng thứ hai thì nó
    // sẽ  nhảy luôn qua scene ở dòng mã thứ 2 (vì không có update nào được chạy trong lúc load ở dòng mã load scene thứ 1 và scene thứ 2 => 
    // màn hình chuyển thẳng qua scene của dòng lệnh thứ 2 => tạo thêm một update để load thêm scene ở dòng lệnh 1)
    private bool isFirstUpdate = true;

    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            Loader.sceneCallback();
        }
    }
}
