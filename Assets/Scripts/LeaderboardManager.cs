using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Analytics;
using System.Threading.Tasks;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int[] levelScores = new int[6];
    public int TotalScore => levelScores.Sum();
}

public class LeaderboardManager : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public Button saveButton;
    public TextMeshProUGUI leaderboardText;

    private async void Awake()
    {
        await InitializeUnityServicesAsync();
    }

    private async Task InitializeUnityServicesAsync()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        }
    }

    private void Start()
    {
        saveButton.onClick.AddListener(async () => await SavePlayerScore());
        _ = ShowTop10Leaderboard();
    }

    async Task SavePlayerScore()
    {
        string name = playerNameInput.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        int[] levelScores = new int[6];
        for (int i = 0; i < 6; i++)
        {
            levelScores[i] = PlayerPrefs.GetInt("Score_Level_" + (i + 1), 0); // ดึงคะแนนที่เคยบันทึกไว้
        }

        int totalScore = levelScores.Sum();

        // 🔁 บันทึกลง Leaderboard
        await LeaderboardsService.Instance.AddPlayerScoreAsync("highscore", totalScore);

        // 📊 ส่งข้อมูล Custom Analytics Event
        CustomEvent myEvemt3 = new CustomEvent("leaderboard_update")
        {
            { "totalScore", totalScore },
            { "playerId", AuthenticationService.Instance.PlayerId }
        };

        AnalyticsService.Instance.RecordEvent(myEvemt3);
        AnalyticsService.Instance.Flush();

        Debug.Log("Score Submitted: " + totalScore);

        await ShowTop10Leaderboard();
    }

    async Task ShowTop10Leaderboard()
    {
        var results = await LeaderboardsService.Instance.GetScoresAsync("highscore", new GetScoresOptions { Limit = 10 });

        string leaderboardDisplay = "Top 10 Leaderboard\n\n";
        int rank = 1;

        foreach (LeaderboardEntry entry in results.Results)
        {
            string name = entry.PlayerName ?? $"Player {rank}";
            leaderboardDisplay += $"{rank++}. {name}: {entry.Score} pts\n";
        }

        leaderboardText.text = leaderboardDisplay;
    }
}
