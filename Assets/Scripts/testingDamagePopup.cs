using UnityEngine;
using UnityEngine.InputSystem;

public class testingDamagePopup : MonoBehaviour
{
    [SerializeField] Transform pfDamagePopup;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int damageAmount = UnityEngine.Random.Range(0, 100);
            DamagePopup.Create(GetMouseWorldPosition(), damageAmount, damageAmount > 70);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        return mouseWorldPosition;

    }
}
