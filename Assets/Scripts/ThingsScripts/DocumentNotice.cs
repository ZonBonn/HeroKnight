using UnityEngine;

public class DocumentNoitice : MonoBehaviour
{
    public GameObject ExclamationMark;
    public GameObject F;
    private bool playerInside;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && playerInside == true)
        {
            // hien UI;
            UICanvasManager.Instance.ShowPannel(UICanvasManager.Instance.instructionKillBossPannel);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && ExclamationMark.activeSelf == false)
        {
            ExclamationMark.SetActive(true);
            F.SetActive(true);
            playerInside = true;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && ExclamationMark.activeSelf == true)
        {
            ExclamationMark.SetActive(false);
            F.SetActive(false);
            playerInside = false;
            UICanvasManager.Instance.HidePannel(UICanvasManager.Instance.instructionKillBossPannel);
        }
    }
}
