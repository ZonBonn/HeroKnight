using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.E) && Input.GetKeyDown(KeyCode.T))
        {
            // SenceController.instance.NextLevel();
            Loader.LoadNextLevel();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") == true)
        {
            // Debug.Log("Load Scene" + SceneManager.GetActiveScene().buildIndex + 1);
            // SenceController.instance.NextLevel();
            Loader.LoadNextLevel();
        }
    }
}
