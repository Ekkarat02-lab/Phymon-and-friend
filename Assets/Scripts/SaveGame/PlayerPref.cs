using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using System;

public class PlayerPref : MonoBehaviour
{
    public int currentStory;
    public static PlayerPref instance;

    private DateTime sessionStartTime;
    private double totalSessionDurationToday = 0;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAnalytics();
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
    
    private async void InitializeAnalytics()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        sessionStartTime = DateTime.Now;
    }

    private void OnApplicationQuit()
    {
        TrackSessionDuration();
    }

    private void TrackSessionDuration()
    {
        double sessionDuration = (DateTime.Now - sessionStartTime).TotalSeconds;
        totalSessionDurationToday += sessionDuration;

        CustomEvent myEvent = new CustomEvent("session_duration")
        {
            { "duration_seconds", sessionDuration },
            { "date", DateTime.Now.ToString("yyyy-MM-dd") }
        };
        
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
        
        CustomEvent myEvent2 = new CustomEvent("daily_playtime")
        {
            { "total_playtime_today_seconds", totalSessionDurationToday },
            { "date", DateTime.Now.ToString("yyyy-MM-dd") }
        };

        AnalyticsService.Instance.RecordEvent(myEvent2);
        AnalyticsService.Instance.Flush();
        
        Debug.Log($"Session duration sent: {sessionDuration} seconds");
        Debug.Log($"Total playtime today sent: {totalSessionDurationToday} seconds");
    }
}
