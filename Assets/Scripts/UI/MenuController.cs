using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // UI สำหรับเมนูหยุดเกม

    private bool isPaused = false;

    void Start()
    {
        // ซ่อนเมนูตอนเริ่มเกม
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    // ฟังก์ชันเล่นเกมต่อ
    public void OnResumeButton()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    // ฟังก์ชันหยุดเกม (เรียกใช้เมื่อกดปุ่ม Pause)
    public void OnPauseButton()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
    }

    // ฟังก์ชันเริ่มเกมใหม่
    public void OnRestartButton()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    // ฟังก์ชันกลับไปหน้า Main Menu
    public void OnMainMenuButton()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }

    // ฟังก์ชันออกจากเกม
    /*public void OnQuitButton()
    {
        Application.Quit(); // ออกจากเกม
        Debug.Log("Game is quitting...");
    }*/
}