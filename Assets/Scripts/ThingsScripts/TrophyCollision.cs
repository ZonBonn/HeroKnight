using UnityEngine;

public class TrophyCollision : MonoBehaviour
{
    private bool isPicked = false;
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag("Player") && !isPicked)
        {
            FunctionTimer.Create(() => { 
                if(UICanvasManager.Instance.endingPannel != null)
                {
                    UICanvasManager.Instance.ShowPannel( UICanvasManager.Instance.endingPannel);
                }
            }, 1f);
            isPicked = true;
            Destroy(gameObject);
        }
    }
}
