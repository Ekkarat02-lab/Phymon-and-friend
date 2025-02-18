using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public RewardManager rewardManager;

    public int totalHearts = 3;
    private int currentHearts;
    public Image[] heartImages;
    public GameObject[] mascotUIImages;

    public GameObject phymonB;
    public GameObject phymonS;
    public GameObject phymonQ;
    public GameObject phymonW;

    public AudioSource audioSource;
    public AudioClip dialogueSound;

    private bool allMascotsFound = false;
    private float mascotMissTimer = 0f;
    private float mascotCheckTimer = 0f;
    private int previousMascotCount;

    private bool isPhymonQActive = false;
    private Coroutine hidePhymonQCoroutine;

    private int currentScore = 0;

    private float startTime;
    
    private void Awake()
    {
        Instance = this;
        currentHearts = totalHearts;
        UpdateHeartUI(); 
        previousMascotCount = mascotUIImages.Length; 
    }
    
    private void Start()
    {
        startTime = Time.time;

        Debug.Log($"[SceneAnalytics] Start Time: {startTime}");

        Button phymonQButton = phymonQ.GetComponent<Button>();
        if (phymonQButton != null)
        {
            phymonQButton.onClick.AddListener(OnPhymonQButtonClick);
        }
        else
        {
            Debug.LogError("phymonQ ไม่มีคอมโพเนนต์ Button!");
        }
    }
    
    private void Update()
    {
        if (!allMascotsFound)
        {
            CheckMissedClick();
            CheckMascotCount();
            CheckAllMascotsFound();
        }
    }

    private void CheckMissedClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mascotMissTimer = 0f;
        }
        else
        {
            mascotMissTimer += Time.deltaTime;

            if (mascotMissTimer >= 5f && !isPhymonQActive)
            {
                ShowPhymonQ();
            }
        }
    }

    private void CheckMascotCount()
    {
        if (mascotUIImages.Length == previousMascotCount)
        {
            mascotCheckTimer += Time.deltaTime;
            if (mascotCheckTimer >= 300f && !isPhymonQActive)
            {
                ShowPhymonQ();
            }
        }
        else
        {
            mascotCheckTimer = 0f;
            previousMascotCount = mascotUIImages.Length;
        }
    }

    private void CheckAllMascotsFound()
    {
        bool allFound = true;

        foreach (var mascot in mascotUIImages)
        {
            if (mascot.activeSelf)
            {
                allFound = false;
                break;
            }
        }

        if (allFound && !allMascotsFound)
        {
            allMascotsFound = true;
            phymonB.SetActive(false);
            phymonW.SetActive(true);
            Debug.Log("หามาสคอตครบแล้ว! แสดง PhymonW");
        }
    }

    private void ShowPhymonQ()
    {
        if (!allMascotsFound)
        {
            phymonQ.SetActive(true);
            phymonB.SetActive(false);
            isPhymonQActive = true;
            Debug.Log("แสดง PhymonQ!");
        }
    }

    public void OnPhymonQButtonClick()
    {
        if (isPhymonQActive)
        {
            HidePhymonQAndShowB();
        }

        PlaySoundOnly();
        Debug.Log("คลิกที่ปุ่ม phymonQ");
    }

    public void HidePhymonQAndShowB()
    {
        if (isPhymonQActive && !allMascotsFound)
        {
            phymonQ.SetActive(false);
            phymonB.SetActive(true);
            isPhymonQActive = false;
            mascotMissTimer = 0f;

            if (hidePhymonQCoroutine != null)
            {
                StopCoroutine(hidePhymonQCoroutine);
            }
            hidePhymonQCoroutine = StartCoroutine(HidePhymonBAfterDelay(4f));
        }
    }

    public void SwitchPhymonQToBWithoutDialogue()
    {
        if (isPhymonQActive)
        {
            phymonQ.SetActive(false);
            phymonB.SetActive(true);
            isPhymonQActive = false;
            Debug.Log("Switched from Phymon Q to Phymon B without dialogue");
        }
    }

    private IEnumerator HidePhymonBAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        phymonB.SetActive(false);
    }

    public void HideMascotOnUI(int mascotIndex)
    {
        if (mascotIndex >= 0 && mascotIndex < mascotUIImages.Length)
        {
            mascotUIImages[mascotIndex].SetActive(false); 
            Debug.Log("ซ่อน Mascot UI ที่ดัชนี: " + mascotIndex);

            HidePhymonQAndShowB();
        }
        else
        {
            Debug.LogError("ดัชนีของมาสคอตไม่ถูกต้อง: " + mascotIndex);
        }
    }

    public void ShowPhymonSForDuration()
    {
        if (!allMascotsFound)
        {
            phymonB.SetActive(false);
            phymonS.SetActive(true);
            StartCoroutine(HidePhymonSAfterDelay(3f)); 
        }
    }

    private IEnumerator HidePhymonSAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        phymonS.SetActive(false); 
        phymonB.SetActive(true); 
    }

    public void AddScore(int scoreAmount)
    {
        currentScore += scoreAmount; 
        Debug.Log("เพิ่มคะแนน: " + scoreAmount + " | คะแนนรวม: " + currentScore);
    }

    public void GiveReward(int rewardAmount)
    {
        AddScore(rewardAmount);
        rewardManager.GiveReward(rewardAmount);
    }

    public void UnlockNextLevel()
    {
        Debug.Log("ระดับถัดไปปลดล็อก!");
    }

    public void GameOver()
    {
        Debug.Log("เกมจบ!");
        ResetHearts();  // รีเซ็ตหัวใจ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < currentHearts;
        }
    }

    public void ResetHearts()
    {
        currentHearts = totalHearts;
        UpdateHeartUI();
    }

    public void LoseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            Debug.Log("เสียหัวใจ! หัวใจเหลือ: " + currentHearts);
            UpdateHeartUI();

            if (currentHearts <= 0)
            {
                GameOver();
            }
            else
            {
                ShowPhymonSForDuration();
            }
        }
    }

    public bool IsPhymonQActive()
    {
        return isPhymonQActive;
    }

    public void SwitchPhymonQToS()
    {
        if (isPhymonQActive)
        {
            StopCoroutine("ShowPhymonSAfterVirusClick");
            StartCoroutine(ShowPhymonSAfterVirusClick());
        }
    }

    private IEnumerator ShowPhymonSAfterVirusClick()
    {
        phymonQ.SetActive(false);
        phymonS.SetActive(true);
        isPhymonQActive = false;

        yield return new WaitForSeconds(2f);

        phymonS.SetActive(false); 
        phymonB.SetActive(true);
    }
    
    public void PlaySoundOnly()
    {
        if (audioSource != null && dialogueSound != null)
        {
            audioSource.clip = dialogueSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is null!");
        }
    }

}