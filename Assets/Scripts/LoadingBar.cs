using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        // Debug.Log("Loading Progress %: " + Loader.getLoadProgress());
        image.fillAmount = Loader.getLoadProgress();
    }
}
