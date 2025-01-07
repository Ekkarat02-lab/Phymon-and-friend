using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveItem : MonoBehaviour
{
    public static bool isDraggingItem = false; // สถานะการลาก item
    private static GameObject currentlyDragging;
    private bool isDragging = false;
    private Vector3 offset;
    private GameObject lastDraggedItem; // เก็บ Item ที่ถูกลากครั้งล่าสุด
    private static Dictionary<GameObject, Vector3> itemOriginalPositions = new Dictionary<GameObject, Vector3>(); // เก็บตำแหน่งเดิมของแต่ละ Item
    private static Dictionary<GameObject, bool> itemReturning = new Dictionary<GameObject, bool>(); // ตรวจสอบว่า item กำลังเลื่อนกลับตำแหน่งเดิมหรือไม่
    private static Dictionary<GameObject, float> itemDragTimes = new Dictionary<GameObject, float>(); // บันทึกเวลาแต่ละ item ถูกลาก
    
    public LayerMask itemLayer; // กำหนด Layer ที่ต้องการ

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast เฉพาะ Layer ที่กำหนด
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, itemLayer);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;

                // ตรวจสอบว่าไม่ใช่ Mascot
                if (clickedObject.CompareTag("Mascot"))
                {
                    return; // ข้าม Mascot ทันที
                }

                // ทำงานเฉพาะกับ Item
                if (clickedObject.CompareTag("Item"))
                {
                    if (!itemReturning.ContainsKey(clickedObject) || !itemReturning[clickedObject])
                    {
                        currentlyDragging = clickedObject;
                        isDragging = true;
                        isDraggingItem = true;

                        offset = currentlyDragging.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        if (!itemOriginalPositions.ContainsKey(currentlyDragging))
                        {
                            itemOriginalPositions[currentlyDragging] = currentlyDragging.transform.position;
                        }

                        itemDragTimes[currentlyDragging] = Time.time;
                    }
                }
            }
        }

        if (isDragging && currentlyDragging != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentlyDragging.transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, currentlyDragging.transform.position.z);

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                isDraggingItem = false;

                lastDraggedItem = currentlyDragging;

                StartCoroutine(DelayedReturnToOriginalPosition(lastDraggedItem));
                currentlyDragging = null;
            }
        }
    }
    
    // Coroutine สำหรับนับเวลาล่าช้า ก่อนจะให้ item กลับตำแหน่งเดิม
    private IEnumerator DelayedReturnToOriginalPosition(GameObject item)
    {
        // ตรวจสอบเวลาที่ถูกลากและตั้งเวลารอให้ item กลับ
        yield return new WaitForSeconds(5f);

        // เริ่มกระบวนการเลื่อนกลับไปตำแหน่งเดิม
        ReturnToOriginalPosition(item);
    }

    // ฟังก์ชันสำหรับกลับไปยังตำแหน่งเดิม
    private void ReturnToOriginalPosition(GameObject item)
    {
        if (item != null && itemOriginalPositions.ContainsKey(item))
        {
            itemReturning[item] = true; // ตั้งสถานะว่า item กำลังคืนตำแหน่ง
            StartCoroutine(MoveBackCoroutine(item)); // เริ่ม Coroutine สำหรับการเคลื่อนที่กลับ
        }
    }

    // Coroutine สำหรับการเลื่อน item กลับตำแหน่งเดิม
    private IEnumerator MoveBackCoroutine(GameObject item)
    {
        Vector3 startPosition = item.transform.position;
        Vector3 endPosition = itemOriginalPositions[item];
        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        while (Vector3.Distance(item.transform.position, endPosition) > 0.1f)
        {
            float distanceCovered = (Time.time - startTime) * 2f; // ปรับ speed ตามต้องการ
            float fractionOfJourney = distanceCovered / journeyLength;

            item.transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
            yield return null;
        }

        item.transform.position = endPosition; // ตั้งตำแหน่งสุดท้ายให้ตรงตำแหน่งเดิม
        itemReturning[item] = false; // เมื่อเสร็จสิ้นการคืนตำแหน่ง
    }
}