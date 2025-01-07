using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MouseClickHandler : MonoBehaviour
{
    public LevelManager levelManager;

    private int virusClickCount = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            { 
                return; 
            }
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Mascot"))
                {
                    Mascot mascot = hit.collider.GetComponent<Mascot>();
                    if (mascot != null)
                    {
                        levelManager.FoundMascot(mascot.mascotIndex);
                        GameManager.Instance.HideMascotOnUI(mascot.mascotIndex);
                        AudioManager.Instance.PlayClickMascotSound();
                        StartCoroutine(DestroyMascotWithDelay(mascot.gameObject, 3f));

                        if (GameManager.Instance.IsPhymonQActive())
                        {
                            GameManager.Instance.SwitchPhymonQToBWithoutDialogue();
                        }
                    }
                }
                else if (hit.collider.CompareTag("Virus"))
                {
                    if (GameManager.Instance.IsPhymonQActive())
                    {
                        GameManager.Instance.SwitchPhymonQToBWithoutDialogue();
                        virusClickCount++;
                    }
                    else
                    {
                        GameManager.Instance.LoseHeart();
                        GameManager.Instance.ShowPhymonSForDuration();
                    }
                        AudioManager.Instance.PlayClickButtonSound();
                }
                /*else if (hit.collider.CompareTag("Get Item"))
                {
                    // คลิก Item และทำลาย
                    Debug.Log("Item clicked!");
                    Destroy(hit.collider.gameObject);

                    // เพิ่ม Logic ที่เกี่ยวข้องกับการเก็บไอเท็ม เช่น อัพเดตจำนวนที่เก็บได้
                    GameManager.Instance.CollectItem(); // สมมติว่า GameManager มีฟังก์ชันนี้
                }
                else
                {
                    Debug.Log($"Clicked on non-handled object: {hit.collider.name}");
                }*/
            }
            else
            {
                AudioManager.Instance?.PlayClickButtonSound();
            }
        }
    }

    private IEnumerator DestroyMascotWithDelay(GameObject mascot, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(mascot);
    }
}