using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveItem : MonoBehaviour
{
    public static bool isDraggingItem = false;
    private static GameObject currentlyDragging;
    private bool isDragging = false;
    private Vector3 offset;
    private GameObject lastDraggedItem;
    private static Dictionary<GameObject, Vector3> itemOriginalPositions = new Dictionary<GameObject, Vector3>(); 
    private static Dictionary<GameObject, bool> itemReturning = new Dictionary<GameObject, bool>();
    private static Dictionary<GameObject, float> itemDragTimes = new Dictionary<GameObject, float>(); 
    
    public LayerMask itemLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, itemLayer);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject.CompareTag("Mascot"))
                {
                    return;
                }

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

    private IEnumerator DelayedReturnToOriginalPosition(GameObject item)
    {
        yield return new WaitForSeconds(5f);

        ReturnToOriginalPosition(item);
    }

    private void ReturnToOriginalPosition(GameObject item)
    {
        if (item != null && itemOriginalPositions.ContainsKey(item))
        {
            itemReturning[item] = true;
            StartCoroutine(MoveBackCoroutine(item));
        }
    }

    private IEnumerator MoveBackCoroutine(GameObject item)
    {
        Vector3 startPosition = item.transform.position;
        Vector3 endPosition = itemOriginalPositions[item];
        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        while (Vector3.Distance(item.transform.position, endPosition) > 0.1f)
        {
            float distanceCovered = (Time.time - startTime) * 2f;
            float fractionOfJourney = distanceCovered / journeyLength;

            item.transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
            yield return null;
        }

        item.transform.position = endPosition;
        itemReturning[item] = false;
    }
}