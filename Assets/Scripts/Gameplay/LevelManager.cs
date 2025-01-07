using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public GameObject[] mascotPrefabs; // Prefab ของมาสคอต
    public Transform[] spawnPoints;   // จุด Spawn ของมาสคอต
    public GameObject indicatorArrowPrefab; // Prefab สำหรับลูกศร
    public QuestionManager questionManager;
    public int rewardAmount = 10;

    private GameObject[] mascots;
    private int foundMascots = 0;

    void Start()
    {
        CreateInitialMascots();
    }

    void CreateInitialMascots()
    {
        // ตรวจสอบว่ามีจุด Spawn และ Prefab เพียงพอ
        if (spawnPoints.Length < 5 || mascotPrefabs.Length < 5)
        {
            Debug.LogError("Insufficient spawn points or mascot prefabs.");
            return;
        }

        mascots = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            // สร้างมาสคอตที่แต่ละตำแหน่ง
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
        
        // ลบมาสคอตในตำแหน่งที่เจอ
        if (mascots[mascotIndex] != null)
        {
            mascots[mascotIndex] = null;
        }

        if (foundMascots == 3)
        {
            if (questionManager != null)
            {
                questionManager.ShowQuestion();
            }
        }
        else if (foundMascots == 5) // เมื่อพบครบ 5 ตัว
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
            // ตรวจสอบว่ามาสคอตยังไม่ได้ถูกหาเจอหรือทำลาย
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
