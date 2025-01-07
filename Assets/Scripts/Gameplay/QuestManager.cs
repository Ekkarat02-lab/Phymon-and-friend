using UnityEngine;
using System.Collections;

public class QuestManager : MonoBehaviour
{
    /*public GameObject[] questItems; // ไอเท็มทั้งหมดที่ต้องเก็บ
    public GameObject arrowIndicatorPrefab; // Prefab ของลูกศรที่จะแสดงตำแหน่งมาสคอต
    public float arrowDisplayDuration = 3f; // ระยะเวลาที่จะแสดงลูกศร
    public LevelManager levelManager; // อ้างอิงถึง LevelManager

    private int collectedItems = 0;

    void Start()
    {
        if (arrowIndicatorPrefab == null)
        {
            Debug.LogError("Arrow Indicator Prefab is not assigned!");
        }
    }

    public void CollectItem(GameObject item)
    {
        if (System.Array.Exists(questItems, element => element == item))
        {
            collectedItems++;
            Destroy(item); // ลบไอเท็มหลังจากเก็บ
            Debug.Log($"Collected items: {collectedItems}/{questItems.Length}");

            if (collectedItems == questItems.Length)
            {
                OnQuestComplete();
            }
        }
    }

    private void OnQuestComplete()
    {
        Debug.Log("All items collected! Spawning arrows at mascot positions.");

        // ค้นหามาสคอตที่ยังไม่ได้ถูกทำลาย
        GameObject[] mascots = GameObject.FindGameObjectsWithTag("Mascot");
        foreach (GameObject mascot in mascots)
        {
            if (mascot.activeSelf) // ตรวจสอบว่ามาสคอตยังอยู่ในเกม
            {
                SpawnArrowAtMascot(mascot);
            }
        }
    }

    private void SpawnArrowAtMascot(GameObject mascot)
    {
        // สร้างลูกศรในตำแหน่งของมาสคอต
        Vector3 spawnPosition = mascot.transform.position + Vector3.up * 1.5f; // ปรับตำแหน่งเพื่อให้ลูกศรไม่ทับมาสคอต
        Debug.Log($"Spawning arrow at position: {spawnPosition}");

        // ตรวจสอบว่า prefab ถูกตั้งค่าอย่างถูกต้อง
        if (arrowIndicatorPrefab != null)
        {
            GameObject arrow = Instantiate(arrowIndicatorPrefab, spawnPosition, Quaternion.identity);
            arrow.transform.SetParent(mascot.transform); // ผูกลูกศรเข้ากับมาสคอต
            Debug.Log("Arrow spawned successfully at " + spawnPosition);

            // ซ่อนลูกศรหลังจากครบเวลา
            StartCoroutine(HideArrowAfterDelay(arrow, arrowDisplayDuration));
        }
        else
        {
            Debug.LogError("Arrow indicator prefab is not assigned!");
        }
    }

    private IEnumerator HideArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(arrow); // ลบลูกศรออกจากเกม
        Debug.Log("Arrow indicator hidden.");
    }*/
}
