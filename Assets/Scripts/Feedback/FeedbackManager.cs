using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class FeedbackManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button submitButton;
    public TMP_Text feedbackStatusText;  // เพิ่ม TMP_Text เพื่อแสดงข้อความสถานะ

    private int submitButtonClickCount = 0;
    private bool isFirebaseReady = false;


    void Start()
    {
        submitButton.onClick.AddListener(SubmitFeedback);
    }

    public void SubmitFeedback()
    {
        if (!isFirebaseReady)
        {
            Debug.LogError("Firebase is not initialized. Cannot log feedback.");
            return;
        }

        string feedback = inputField.text;

        if (!string.IsNullOrEmpty(feedback))
        {
            Debug.Log($"Sending Feedback: {feedback}");

            submitButtonClickCount++;

            // Save feedback to local file
            SaveFeedbackToFile(feedback);

            inputField.text = "";
        }
        else
        {
            Debug.Log("Feedback is empty!");

            // แสดงข้อความเมื่อไม่มีฟีดแบ็ก
            feedbackStatusText.text = "Please enter some feedback.";
            feedbackStatusText.color = Color.red;
        }
    }
    private void SaveFeedbackToFile(string feedback)
    {
        string filePath = Application.persistentDataPath + "/FeedbackLog.txt";

        try
        {
            string logEntry = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {feedback}\n";
            File.AppendAllText(filePath, logEntry);
            Debug.Log($"Feedback saved to file: {filePath}");

            // แสดงข้อความใน UI ว่าฟีดแบ็กถูกบันทึกแล้วที่ไหน
            feedbackStatusText.text = $"Feedback saved at: {filePath}";
            feedbackStatusText.color = Color.blue; // ตั้งสีข้อความให้เป็นสีฟ้า
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving feedback to file: {ex.Message}");

            // แสดงข้อความใน UI หากเกิดข้อผิดพลาด
            feedbackStatusText.text = "Error saving feedback. Please try again.";
            feedbackStatusText.color = Color.red; // ตั้งสีข้อความให้เป็นสีแดง
        }
    }

}
