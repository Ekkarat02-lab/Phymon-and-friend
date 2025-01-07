using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key; // คีย์ข้อความจากไฟล์ JSON
    private Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<Text>();
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance != null)
        {
            textComponent.text = LocalizationManager.Instance.GetLocalizedValue(key);
        }
    }

    private void OnEnable()
    {
        UpdateText(); // อัปเดตข้อความเมื่อเปิดใช้งาน GameObject
    }
}