using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    public void OnResumeButton()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    public void OnPauseButton()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }
}