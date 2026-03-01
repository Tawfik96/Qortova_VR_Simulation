
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // change key if you want
        {
            Time.timeScale = 1f; // reset timescale before loading
            SceneManager.LoadScene(0); // 0 = MainMenu
        }
    }
}