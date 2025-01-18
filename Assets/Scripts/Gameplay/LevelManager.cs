using UnityEngine; 
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public GameObject[] mascotPrefabs;
    public Transform[] spawnPoints;
    public GameObject indicatorArrowPrefab;
    public QuestionManager questionManager;
    public int rewardAmount = 10;

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
        }
    }

    public void FoundMascot(int mascotIndex)
    {
        foundMascots++;
        Debug.Log("Mascot Found! Current count: " + foundMascots);
        AudioManager.Instance.PlayClickMascotSound();

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
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnlockNextLevel();
                GameManager.Instance.GiveReward(rewardAmount);
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
}