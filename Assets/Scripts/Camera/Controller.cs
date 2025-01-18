using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float dragSpeed = 2f;
    public float zoomSpeed = 0.1f; // ลดความเร็วของการซูมสำหรับการสัมผัส
    public float smoothTime = 0.2f; // ความสมูทของการเคลื่อนไหว
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public Vector2 dragLimitMin;
    public Vector2 dragLimitMax;

    private Vector3 dragOrigin;
    private Camera cam;
    private Vector3 targetPosition;
    private float targetZoom;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        cam = Camera.main;
        targetPosition = transform.position;
        targetZoom = cam.orthographicSize;
    }

    private void Update()
    {
        if (Input.touchCount == 1) // ลากด้วยนิ้วเดียว
        {
            DragCamera();
        }
        else if (Input.touchCount == 2) // ซูมด้วยสองนิ้ว
        {
            ZoomCamera();
        }

        // Smooth Movement
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Smooth Zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime / smoothTime);
    }

    void DragCamera()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            dragOrigin = touch.position;
        }

        if (touch.phase == TouchPhase.Moved)
        {
            Vector3 difference = dragOrigin - (Vector3)touch.position;
            dragOrigin = touch.position;

            Vector3 move = new Vector3(difference.x * dragSpeed * Time.deltaTime, difference.y * dragSpeed * Time.deltaTime, 0);
            targetPosition += move;

            // Clamp the target position
            targetPosition.x = Mathf.Clamp(targetPosition.x, dragLimitMin.x, dragLimitMax.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, dragLimitMin.y, dragLimitMax.y);
        }
    }

    void ZoomCamera()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // ตรวจสอบการสัมผัสทั้งสองนิ้ว
        if (touchZero.phase == TouchPhase.Moved && touchOne.phase == TouchPhase.Moved)
        {
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            targetZoom -= difference * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }
}