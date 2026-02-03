using UnityEngine;

public class Stone : MonoBehaviour
{
    private Transform symbolTransform;

    private void Start()
    {
        symbolTransform = gameObject.transform.Find("StoneSymbol");
    }

    public void shouldActiveStoneSymbol(bool shouldActive)
    {
        symbolTransform.gameObject.SetActive(shouldActive);
    }
}
