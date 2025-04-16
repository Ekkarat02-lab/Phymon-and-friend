using UnityEngine; 
using System.Collections;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject[] mascotPrefabs;
    public Transform[] spawnPoints;
    public GameObject indicatorArrowPrefab;
    public QuestionManager questionManager;
    public int rewardAmount = 10;
    public TextMeshProUGUI scoreText;
    
    private float[] mascotStartTimes = new float[5];
    private int score = 0;
    private GameObject[] mascots;
    private int foundMascots = 0;
    private int targetMascotsToTriggerQuestion;

    void Start()
    {
        targetMascotsToTriggerQuestion = Random.Range(1, 4);
        Debug.Log("Target mascots to trigger question: " + targetMascotsToTriggerQuestion);
        CreateInitialMascots();
    }

    void CreateInitialMascots()
    {
        if (spawnPoints.Length < 5 || mascotPrefabs.Length < 5)
        {
            Debug.LogError("Insufficient spawn points or mascot prefabs.");
            return;
        }

        mascots = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            GameObject mascot = Instantiate(mascotPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            mascot.GetComponent<Mascot>().levelManager = this;
            mascot.GetComponent<Mascot>().mascotIndex = i;
            mascots[i] = mascot;
            mascotStartTimes[i] = Time.time;
        }
    }

    public void FoundMascot(int mascotIndex)
    {
        foundMascots++;
        Debug.Log("Mascot Found! Current count: " + foundMascots);
        AudioManager.Instance.PlayClickMascotSound();

        float timeTaken = Time.time - mascotStartTimes[mascotIndex];
        int points = 0;

        if (timeTaken <= 5f)
            points = 50;
        else if (timeTaken <= 7f)
            points = 35;
        else if (timeTaken <= 10f)
            points = 25;

        int targetScore = score + points;
        StartCoroutine(AnimateScore(targetScore));
        
        // รีเซ็ตเวลาให้ mascot ตัวอื่นที่ยังไม่ถูกคลิก
        for (int i = 0; i < mascots.Length; i++)
        {
            if (mascots[i] != null && i != mascotIndex)
            {
                mascotStartTimes[i] = Time.time;
            }
        }

        if (mascots[mascotIndex] != null)
        {
            mascots[mascotIndex] = null;
        }

        if (foundMascots == targetMascotsToTriggerQuestion)
        {
            if (questionManager != null)
            {
                questionManager.ShowQuestion();
            }
        }

        if (foundMascots == 5)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayWinnerSound();
            }
            // หลังจากให้รางวัลและปลดล็อคด่านใหม่ (ใน FoundMascot)
            // หลังจากให้รางวัลและปลดล็อคด่านใหม่ (ใน FoundMascot)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnlockNextLevel();
                GameManager.Instance.GiveReward(rewardAmount);
    
                // เพิ่มบรรทัดนี้เพื่อเก็บคะแนนของด่านนี้
                PlayerPrefs.SetInt("Score_Level_" + PlayerPref.instance.currentStory, score);
            }

            Debug.Log("All mascots collected!");
        }
    }

    public void ShowIndicatorArrows()
    {
        for (int i = 0; i < mascots.Length; i++)
        {
            if (mascots[i] != null)
            {
                GameObject arrow = Instantiate(indicatorArrowPrefab, mascots[i].transform.position, Quaternion.identity);
                arrow.SetActive(true);
                StartCoroutine(HideArrowAfterDelay(arrow, 3f));
            }
        }
    }

    private IEnumerator HideArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (arrow != null)
        {
            Destroy(arrow);
        }
    }
    
    private IEnumerator AnimateScore(int targetScore)
    {
        while (score < targetScore)
        {
            score++;
            if (scoreText != null)
                scoreText.text = "Score: " + score;
            yield return new WaitForSeconds(0.02f); // ความเร็วในการเพิ่มคะแนน
        }
    }

}