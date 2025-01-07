using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    
    public static MainMenuManager Instance;
    
    public StoryManager storyManager;
    public GameObject mainMenu;
    public GameObject selectStoryMenu;
    public GameObject extraMenu;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void StartGameButton()
    {
        mainMenu.SetActive(false);
        selectStoryMenu.SetActive(true);
    }

    public void BackButton()
    {
        selectStoryMenu.SetActive(false);
        extraMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ClearSaveButton()
    {
        storyManager.ClearSaveData();
    }

    public void ExtraButton()
    {
        mainMenu.SetActive(false);
        extraMenu.SetActive(true);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
