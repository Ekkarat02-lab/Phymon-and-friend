using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager instance;
    
    public Button[] stageButtons;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        UpdateStageButtons();
    }

    private void UpdateStageButtons()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int storyID = i + 1;
            bool isCleared = PlayerPref.instance.IsStoryCleared(storyID);

            if (isCleared || storyID == 1 || PlayerPref.instance.IsStoryCleared(storyID - 1))
            {
                stageButtons[i].interactable = true;
                stageButtons[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                stageButtons[i].interactable = false;
                stageButtons[i].GetComponent<Image>().color = Color.gray;
            }

            int storyIndex = storyID;
            stageButtons[i].onClick.AddListener(() => LoadStory(storyIndex));
        }
    }

    public void LoadStory(int storyID)
    {
        string sceneName = "StoryGame " + storyID;
        PlayerPref.instance.currentStory = storyID;
        SceneManager.LoadScene(sceneName);
    }
    
    public void ClearSaveData()
    {
        PlayerPref.instance.ClearAllSavedStory();
        UpdateStageButtons();
    }
    
}