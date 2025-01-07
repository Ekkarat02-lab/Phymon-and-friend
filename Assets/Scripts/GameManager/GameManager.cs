using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton instance เพื่อเข้าถึง GameManager จากสคริปต์อื่นๆ
    public static GameManager Instance;

    // อ้างอิงถึง RewardManager สำหรับการจัดการรางวัล
    public RewardManager rewardManager;

    // ตัวแปรสำหรับการจัดการจำนวนหัวใจ
    public int totalHearts = 3;
    private int currentHearts;
    public Image[] heartImages;  // UI รูปภาพหัวใจ
    public GameObject[] mascotUIImages;  // UI รูปภาพมาสคอต

    // GameObjects ที่แสดงสถานะต่างๆ ของมาสคอต Phymon
    public GameObject phymonB; // Phymon เริ่มต้น
    public GameObject phymonS; // เมื่อเสียหัวใจ
    public GameObject phymonQ; // แสดงเมื่อคลิกผิด
    public GameObject phymonW; // เมื่อหามาสคอตครบ

    public AudioSource audioSource;  // อ้างอิงถึง AudioSource
    public AudioClip dialogueSound;  // อ้างอิงถึง AudioClip
    
    // การติดตามสถานะของมาสคอตและตัวจับเวลา
    private bool allMascotsFound = false;
    private float mascotMissTimer = 0f;
    private float mascotCheckTimer = 0f;
    private int previousMascotCount;

    // สถานะการแสดง PhymonQ
    private bool isPhymonQActive = false;
    private Coroutine hidePhymonQCoroutine;

    // ตัวแปรสำหรับคะแนน
    private int currentScore = 0;
    
    //ส่งข้อมูลเข้า firebase console
    private float startTime;
    
    /*private int collectedItems = 0; // จำนวนไอเท็มที่เก็บได้
    public int totalItems = 5; // จำนวนไอเท็มทั้งหมดในเควส
    public TextMeshProUGUI itemsCollectedText;*/
    
    // ฟังก์ชันนี้จะทำงานเมื่อเริ่มต้นสคริปต์
    private void Awake()
    {
        Instance = this;  // กำหนด instance ของ GameManager
        currentHearts = totalHearts;  // กำหนดค่าเริ่มต้นของหัวใจ
        UpdateHeartUI();  // อัปเดต UI หัวใจตอนเริ่มเกม
        previousMascotCount = mascotUIImages.Length;  // บันทึกจำนวนมาสคอตเริ่มต้น
    }
    
    private void Start()
    {
        // เริ่มต้นจับเวลาตั้งแต่เข้ามาใน Scene
        startTime = Time.time;
        // Debug เพื่อดูว่าการจับเวลาเริ่มทำงาน
        Debug.Log($"[SceneAnalytics] Start Time: {startTime}");
        //UpdateItemsCollectedText();
        
        // ตรวจสอบว่ามี Button คอมโพเนนต์หรือไม่
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
        // ตรวจสอบการทำงานต่างๆ ถ้ายังไม่หามาสคอตครบ
        if (!allMascotsFound)
        {
            CheckMissedClick();  // ตรวจสอบว่าคลิกผิดหรือไม่
            CheckMascotCount();  // ตรวจสอบจำนวนมาสคอต
            CheckAllMascotsFound();  // ตรวจสอบว่าหามาสคอตครบหรือยัง
        }
    }

    // ฟังก์ชันตรวจสอบว่าไม่มีการคลิกมาสคอตภายในเวลา 5 วินาที
    private void CheckMissedClick()
    {
        if (Input.GetMouseButtonDown(0))  // เมื่อคลิกเมาส์
        {
            mascotMissTimer = 0f;  // รีเซ็ตตัวจับเวลา
        }
        else
        {
            mascotMissTimer += Time.deltaTime;  // เพิ่มค่าตัวจับเวลา
            // ถ้า 5 วินาทีผ่านไปโดยยังไม่แสดง PhymonQ
            if (mascotMissTimer >= 5f && !isPhymonQActive)
            {
                ShowPhymonQ();
            }
        }
    }

    // ฟังก์ชันตรวจสอบว่าไม่มีการเปลี่ยนแปลงในจำนวนมาสคอตภายในเวลา 5 นาที
    private void CheckMascotCount()
    {
        if (mascotUIImages.Length == previousMascotCount)  // ถ้าจำนวนมาสคอตไม่เปลี่ยนแปลง
        {
            mascotCheckTimer += Time.deltaTime;  // เพิ่มค่าตัวจับเวลา
            // ถ้า 5 นาทีผ่านไปโดยไม่มีการเปลี่ยนแปลงในจำนวนมาสคอต
            if (mascotCheckTimer >= 300f && !isPhymonQActive)
            {
                ShowPhymonQ();
            }
        }
        else
        {
            mascotCheckTimer = 0f;  // รีเซ็ตตัวจับเวลาเมื่อจำนวนมาสคอตเปลี่ยนแปลง
            previousMascotCount = mascotUIImages.Length;
        }
    }

    // ฟังก์ชันตรวจสอบว่าหามาสคอตครบแล้วหรือยัง
    private void CheckAllMascotsFound()
    {
        bool allFound = true;

        foreach (var mascot in mascotUIImages)  // ตรวจสอบมาสคอตแต่ละตัวใน UI
        {
            if (mascot.activeSelf)  // ถ้ามาสคอตตัวใดยังแสดงอยู่
            {
                allFound = false;  // ยังไม่ครบ
                break;
            }
        }

        // ถ้าหามาสคอตครบแล้ว แสดง PhymonW
        if (allFound && !allMascotsFound)
        {
            allMascotsFound = true;
            phymonB.SetActive(false);  // ซ่อน PhymonB
            phymonW.SetActive(true);  // แสดง PhymonW
            Debug.Log("หามาสคอตครบแล้ว! แสดง PhymonW");
        }
    }

    // แสดง PhymonQ เมื่อผู้เล่นคลิกผิด
    private void ShowPhymonQ()
    {
        if (!allMascotsFound)  // ถ้ายังไม่หามาสคอตครบ
        {
            phymonQ.SetActive(true);  // แสดง PhymonQ
            phymonB.SetActive(false);  // ซ่อน PhymonB
            isPhymonQActive = true;  // ตั้งค่าสถานะว่า PhymonQ แสดงอยู่
            Debug.Log("แสดง PhymonQ!");
        }
    }

    // ฟังก์ชันที่จะถูกเรียกเมื่อคลิกปุ่ม `phymonQ`
    public void OnPhymonQButtonClick()
    {
        if (isPhymonQActive)
        {
            HidePhymonQAndShowB();
        }

        PlaySoundOnly();
        Debug.Log("คลิกที่ปุ่ม phymonQ");
    }
    
    // ฟังก์ชัน HidePhymonQAndShowB ที่อัปเดต
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
    
    // ฟังก์ชันเพื่อเปลี่ยนจาก phymonQ ไปยัง phymonB โดยไม่แสดง dialogue
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

    // Coroutine เพื่อซ่อน PhymonB หลังจากดีเลย์
    private IEnumerator HidePhymonBAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // รอจนกว่าดีเลย์จะหมด
        phymonB.SetActive(false);  // ซ่อน PhymonB
    }

    // ซ่อนมาสคอตใน UI ตามดัชนีที่ระบุ
    public void HideMascotOnUI(int mascotIndex)
    {
        if (mascotIndex >= 0 && mascotIndex < mascotUIImages.Length)  // ตรวจสอบดัชนีว่าอยู่ในขอบเขต
        {
            mascotUIImages[mascotIndex].SetActive(false);  // ซ่อนมาสคอต
            Debug.Log("ซ่อน Mascot UI ที่ดัชนี: " + mascotIndex);

            HidePhymonQAndShowB();  // รีเซ็ตการแสดง Phymon
        }
        else
        {
            Debug.LogError("ดัชนีของมาสคอตไม่ถูกต้อง: " + mascotIndex);  // จัดการกรณีดัชนีไม่ถูกต้อง
        }
    }

    // แสดง PhymonS ชั่วคราวเมื่อเสียหัวใจ
    public void ShowPhymonSForDuration()
    {
        if (!allMascotsFound)  // ถ้ายังไม่หามาสคอตครบ
        {
            phymonB.SetActive(false);  // ซ่อน PhymonB
            phymonS.SetActive(true);  // แสดง PhymonS
            StartCoroutine(HidePhymonSAfterDelay(3f));  // ซ่อน PhymonS หลังจาก 3 วินาที
        }
    }

    // Coroutine เพื่อซ่อน PhymonS หลังจากดีเลย์
    private IEnumerator HidePhymonSAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        phymonS.SetActive(false);  // ซ่อน PhymonS
        phymonB.SetActive(true);  // แสดง PhymonB
    }

    // เพิ่มคะแนนเมื่อผู้เล่นทำภารกิจสำเร็จ
    public void AddScore(int scoreAmount)
    {
        currentScore += scoreAmount;  // เพิ่มคะแนน
        Debug.Log("เพิ่มคะแนน: " + scoreAmount + " | คะแนนรวม: " + currentScore);
    }

    // ให้รางวัลและเพิ่มคะแนน
    public void GiveReward(int rewardAmount)
    {
        AddScore(rewardAmount);  // เพิ่มคะแนน
        rewardManager.GiveReward(rewardAmount);  // แจ้ง RewardManager ให้แจกของรางวัล
    }

    // เปิดระดับถัดไป
    public void UnlockNextLevel()
    {
        Debug.Log("ระดับถัดไปปลดล็อก!");
    }

    // ฟังก์ชันจบเกม
    public void GameOver()
    {
        Debug.Log("เกมจบ!");
        ResetHearts();  // รีเซ็ตหัวใจ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // โหลดซีนปัจจุบันใหม่
    }

    // อัปเดต UI หัวใจ
    private void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < currentHearts;  // แสดงหรือซ่อนหัวใจตามจำนวน
        }
    }

    // รีเซ็ตหัวใจ
    public void ResetHearts()
    {
        currentHearts = totalHearts;
        UpdateHeartUI();
    }

    // ฟังก์ชันเมื่อเสียหัวใจ
    public void LoseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            Debug.Log("เสียหัวใจ! หัวใจเหลือ: " + currentHearts);
            UpdateHeartUI();

            if (currentHearts <= 0)
            {
                GameOver();  // ถ้าไม่มีหัวใจเหลือให้จบเกม
            }
            else
            {
                ShowPhymonSForDuration();  // แสดง PhymonS เมื่อเสียหัวใจ
            }
        }
    }
    
    // ตรวจสอบว่า phymonQ กำลังแสดงอยู่หรือไม่
    public bool IsPhymonQActive()
    {
        return isPhymonQActive;
    }

    // ซ่อน phymonQ แล้วแสดง phymonS เป็นเวลา 2 วินาที จากนั้นซ่อน phymonS และแสดง phymonB
    public void SwitchPhymonQToS()
    {
        if (isPhymonQActive)
        {
            StopCoroutine("ShowPhymonSAfterVirusClick");
            StartCoroutine(ShowPhymonSAfterVirusClick());
        }
    }

    // Coroutine เพื่อจัดการแสดงผลตามที่กำหนด
    private IEnumerator ShowPhymonSAfterVirusClick()
    {
        phymonQ.SetActive(false);   // ซ่อน phymonQ
        phymonS.SetActive(true);    // แสดง phymonS
        isPhymonQActive = false;

        yield return new WaitForSeconds(2f); // รอ 2 วินาที

        phymonS.SetActive(false);   // ซ่อน phymonS
        phymonB.SetActive(true);    // แสดง phymonB
    }
    
    public void PlaySoundOnly()
    {
        if (audioSource != null && dialogueSound != null)
        {
            audioSource.clip = dialogueSound;  // กำหนดคลิปเสียง
            audioSource.Play();  // เล่นเสียง
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is null!");  // ตรวจสอบว่า AudioSource หรือ AudioClip เป็น null หรือไม่
        }
    }
    
    /*public void CollectItem()
    {
        collectedItems++;
        Debug.Log($"Items collected: {collectedItems}/{totalItems}");

        // อัปเดต UI ทุกครั้งที่เก็บไอเท็ม
        UpdateItemsCollectedText();

        if (collectedItems >= totalItems)
        {
            Debug.Log("All items collected! Revealing final mascot position.");
            // แสดงตำแหน่งของ mascot ผ่าน UI
            UIManager.Instance.ShowFinalMascotLocation();
        }
    }
    
    private void UpdateItemsCollectedText()
    {
        if (itemsCollectedText != null)
        {
            itemsCollectedText.text = $"Items Collected : {collectedItems}/{totalItems}";
        }
    }*/
}