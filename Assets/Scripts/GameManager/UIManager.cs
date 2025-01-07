using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /*public GameObject finalMascotUI; // อ้างอิงถึง UI ที่ใช้แสดงตำแหน่ง

    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowFinalMascotLocation()
    {
        if (finalMascotUI != null)
        {
            finalMascotUI.SetActive(true); // เปิด UI เพื่อแสดงตำแหน่ง
            Debug.Log("Final mascot location revealed!");

            // เรียกใช้ Coroutine เพื่อซ่อน UI หลังจาก 3 วินาที
            StartCoroutine(HideFinalMascotLocationAfterDelay(3f));
        }
    }

    private IEnumerator HideFinalMascotLocationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // รอเวลาตามที่กำหนด
        if (finalMascotUI != null)
        {
            finalMascotUI.SetActive(false); // ซ่อน UI
            Debug.Log("Final mascot location indicator hidden.");
        }
    }
    public void UpdateItemsCollectedText(int collected, int total)
    {
        if (finalMascotUI != null)
        {
            Text collectedText = finalMascotUI.GetComponentInChildren<Text>();
            if (collectedText != null)
            {
                collectedText.text = $"Items Collected: {collected}/{total}";
            }
        }
    }*/
}