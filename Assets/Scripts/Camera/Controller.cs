using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float dragSpeed = 2f;
    public float zoomSpeed = 5f;
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
        ZoomCamera();
        DragCamera();

        // Smooth Movement
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Smooth Zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime / smoothTime);
    }

    void DragCamera()
    {
        // ป้องกันการเลื่อนกล้องหากผู้เล่นกำลังลากไอเท็มอยู่
        if (MoveItem.isDraggingItem)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - Input.mousePosition;
            dragOrigin = Input.mousePosition;

            Vector3 move = new Vector3(difference.x * dragSpeed * Time.deltaTime, difference.y * dragSpeed * Time.deltaTime, 0);
            targetPosition += move;

            // Clamp the target position
            targetPosition.x = Mathf.Clamp(targetPosition.x, dragLimitMin.x, dragLimitMax.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, dragLimitMin.y, dragLimitMax.y);
        }
    }

    void ZoomCamera()
    {
        // ซูมด้วยการหมุนเมาส์ (ScrollWheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // ซูมด้วยการใช้เมาส์กลาง (Middle Mouse Button)
        if (Input.GetMouseButton(2))  // 2 คือหมายเลขของปุ่มเมาส์กลาง
        {
            float zoomChange = Input.GetAxis("Mouse Y") * zoomSpeed; // ใช้ค่า Mouse Y ในการซูม
            targetZoom -= zoomChange;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // เมื่อซูมออกถึง maxZoom ให้กล้องเลื่อนไปตำแหน่ง (0, 0, -10)
        if (Mathf.Approximately(targetZoom, maxZoom))
        {
            targetPosition = new Vector3(0, 0, -10);
        }
    }
}