using Mono.Cecil;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    // tạo ra một nơi để lấy pf từ bên ngoài không bên scene
    private static GameAssets _i; // check khởi tạo
    public static GameAssets i // == Instance
    {
        get
        {
            if(_i == null) // lần đầu gọi
            {
                _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            }
            return _i;
        }
    }

    public Transform pfDamagePopup;
}
