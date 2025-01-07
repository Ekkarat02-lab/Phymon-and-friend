using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPref : MonoBehaviour
{
    public int currentStory;
    public static PlayerPref instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SaveClearedStory()
    {
        PlayerPrefs.SetInt("StoryCleared_" + currentStory, 1);
        Debug.Log("Story " + currentStory + " has been cleared and saved.");
    }

    public bool IsStoryCleared(int storyID)
    {
        return PlayerPrefs.GetInt("StoryCleared_" + storyID, 0) == 1;
    }
    public void ClearAllSavedStory()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All saved storys have been cleared.");
    }
}
