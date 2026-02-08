using UnityEngine;
using UnityEngine.UI;

public class LoadingTextHandler : MonoBehaviour
{
    private float timerChangeTextString;
    [SerializeField] float timeChangeTextString = 0.1f;
    private string[] LoadingStringText = new string[]{"Loading", "Loading.", "Loading..", "Loading..."};
    private int currentIdx;
    private Text text;

    private void Awake()
    {
        text = gameObject.GetComponent<Text>();
    }

    private void Start()
    {
        currentIdx = 0;
        text.text = LoadingStringText[0];
        timerChangeTextString = timeChangeTextString;
    }

    private void Update()
    {
        timerChangeTextString -= Time.deltaTime;
        if(timerChangeTextString <= 0)
        {
            if(++currentIdx >= LoadingStringText.Length)
            {
                currentIdx = 0;
            }
            text.text = LoadingStringText[currentIdx];
            timerChangeTextString = timeChangeTextString;
        }
    }
}
