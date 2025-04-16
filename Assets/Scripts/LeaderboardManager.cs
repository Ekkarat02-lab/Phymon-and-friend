using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

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

    private List<PlayerData> players = new List<PlayerData>();

    private void Start()
    {
        LoadPlayerData();
        ShowLeaderboard();

        saveButton.onClick.AddListener(SavePlayerScore);
    }

    void SavePlayerScore()
    {
        string name = playerNameInput.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        int[] levelScores = new int[6];
        for (int i = 0; i < 6; i++)
        {
            levelScores[i] = PlayerPrefs.GetInt("Score_Level_" + (i + 1), 0); // เก็บจากคะแนนที่เล่นไว้ก่อนหน้า
        }

        int playerCount = PlayerPrefs.GetInt("PlayerCount", 0);

        PlayerPrefs.SetString($"Player_{playerCount}_Name", name);
        for (int i = 0; i < 6; i++)
        {
            PlayerPrefs.SetInt($"Player_{playerCount}_Score_Level_{i + 1}", levelScores[i]);
        }

        PlayerPrefs.SetInt("PlayerCount", playerCount + 1);
        PlayerPrefs.Save();

        LoadPlayerData();
        ShowLeaderboard();
    }

    void LoadPlayerData()
    {
        players.Clear();
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 0);

        for (int i = 0; i < playerCount; i++)
        {
            string playerName = PlayerPrefs.GetString($"Player_{i}_Name", "Unknown");
            int[] scores = new int[6];
            for (int j = 0; j < 6; j++)
            {
                scores[j] = PlayerPrefs.GetInt($"Player_{i}_Score_Level_{j + 1}", 0);
            }

            players.Add(new PlayerData { playerName = playerName, levelScores = scores });
        }
    }

    void ShowLeaderboard()
    {
        var sorted = players.OrderByDescending(p => p.TotalScore).ToList();
        string[] roundNames = { "Round 1", "Round 2", "Round 3" };

        string leaderboardDisplay = "Leaderboard\n\n";

        for (int i = 0; i < sorted.Count && i < 3; i++)
        {
            leaderboardDisplay += $"{roundNames[i]} - {sorted[i].playerName}: {sorted[i].TotalScore} pts\n";
        }

        leaderboardText.text = leaderboardDisplay;
    }
    
}
