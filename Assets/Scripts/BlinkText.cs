using TMPro;
using UnityEngine;
using System.Collections;

public class BlinkText : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            yield return Fade(0f, 1f);
            yield return new WaitForSeconds(0.05f);
            yield return Fade(1f, 0f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator Fade(float start, float end)
    {
        float time = 0f;
        float duration = 0.8f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(start, end, time / duration); // giá trị nhận từ 0->1 lấy ở giữa theo tỉ lệ kéo dài từ 0-> 0.8f dùng hàm Mathf.Lerp
            SetAlpha(alpha);

            yield return null;
        }
    }

    void SetAlpha(float alpha)
    {
        Color c = text.color; // Color là struct không gán trực tiếp được
        c.a = alpha; // sửa xong gán lại
        text.color = c; // gán lại
    }
}